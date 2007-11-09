using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// Quantum GCM calculated in the basis of the 5D linear harmonic oscilator
    /// (nonrotating case)
    /// </summary>
    public class LHOQuantumGCMI5D: LHOQuantumGCMI {
        // Indexy báze
        protected LHO5DIndex index;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMI5D(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        protected override int GetBasisLength() {
            return this.index.Length;
        }

        protected override int GetBasisQuantumNumber1(int i) {
            if(i < 0)
                return this.index.MaxL + 1;
            else
                return this.index.L[i];
        }

        protected override int GetBasisQuantumNumber2(int i) {
            if(i < 0)
                return this.index.MaxMu + 1;
            else
                return this.index.Mu[i];
        }

        protected override int MaximalNumNodes { get { return this.index.MaxMu; } }
        protected override double MaximalRange { get { return System.Math.Sqrt(this.Hbar * this.Omega * this.index.MaxE / this.A0); } }
        protected override double PsiRange(double range) {
            return this.Psi(range, this.index.MaxL, this.index.MaxMu);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy LHO5DIndex
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        protected virtual void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHO5DIndex(maxE);
        }

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        public override int HamiltonianMatrixSize(int maxE) {
            this.CreateIndex(maxE);
            return this.index.Length;
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            if(writer != null)
                writer.Write(Messages.TraceHM);

            DateTime startTime = DateTime.Now;
            double result = this.HamiltonianSBMatrix(maxE, numSteps, null, true).Trace();

            if(writer != null) {
                writer.Write(result);
                writer.Write(' ');
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
            }

            return result;
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici do tvaru pásové matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        protected override SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return this.HamiltonianSBMatrix(maxE, numSteps, writer, false);
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici do tvaru pásové matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        /// <param name="trace">Calculates only trace of the matrix</param>
        protected virtual SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer, bool trace) {
            this.CreateIndex(maxE);

            if(numSteps == 0)
                numSteps = 10 * this.index.MaxMu + 1;

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine("Hamiltonova matice");
                writer.Indent(1);
                writer.WriteLine(string.Format("Pøipravuji cache potenciálu ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange();

            // Cache psi hodnot (Bazove vlnove funkce)
            DiscreteInterval interval = new DiscreteInterval(0.0, range, numSteps);

            double step = interval.Step;

            // Cache hodnot potencialu
            double[] vCache1 = new double[numSteps];
            double[] vCache2 = new double[numSteps];
            for(int sb = 0; sb < numSteps; sb++) {
                double beta = interval.GetX(sb);
                double beta2 = beta * beta;
                vCache1[sb] = beta2 * beta2 * beta2 * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = this.B * beta2 * beta2 * beta2 * beta;
            }

            int length = this.index.Length;
            int bandWidth = trace ? 0 : maxE;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            int blockSize = bandWidth + 1;
            int blockNum = length / blockSize + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("Pøíprava pásové matice H ({0} x {1})", length, maxE - 2));
                writer.WriteLine(string.Format("Poèet blokù: {0}, velikost bloku: {1}", blockNum, blockSize));
            }

            DateTime startTime1 = DateTime.Now;

            BasisCache cache2 = new BasisCache(interval, 0, System.Math.Min(blockSize, this.index.Length), this.Psi);
            BasisCache cache1 = cache2;

            for(int k = 0; k < blockNum; k++) {
                int i0 = k * blockSize;
                int i1 = System.Math.Min((k + 1) * blockSize, this.index.Length);
                int i2 = System.Math.Min((k + 2) * blockSize, this.index.Length);

                if(writer != null) {
                    writer.Write(k);
                    writer.Write(" D");
                }

                // Diagonal block
                for(int i = i0; i < i1; i++) {
                    int li = this.index.L[i];
                    int mui = this.index.Mu[i];

                    for(int j = i; j < i1; j++) {
                        int lj = this.index.L[j];
                        int muj = this.index.Mu[j];

                        // Selection rule
                        if(mui != muj && System.Math.Abs(mui - muj) != 1)
                            continue;

                        // If only trace
                        if(trace && i != j)
                            continue;

                        double sum = 0;

                        double[] vCache = vCache2;
                        if(mui == muj)
                            vCache = vCache1;

                        for(int sb = 0; sb < numSteps; sb++)
                            sum += cache1[i, sb] * vCache[sb] * cache1[j, sb];

                        sum *= step;

                        if(mui != muj) {
                            int mu = System.Math.Max(mui, muj);
                            sum *= mu / System.Math.Sqrt((2.0 * mu - 1.0) * (2.0 * mu + 1.0));
                        }

                        if(mui == muj && li == lj)
                            sum += this.Hbar * omega * (2.5 + li + li + 3 * mui);

                        // Již je symetrické
                        m[i, j] = sum;
                    }
                }

                if(writer != null)
                    writer.Write('C');

                cache2 = new BasisCache(interval, i1, i2, this.Psi);

                // If not only trace
                if(!trace) {
                    if(writer != null)
                        writer.Write("N ");

                    // Nediagonální blok
                    for(int i = i0; i < i1; i++) {
                        int li = this.index.L[i];
                        int mui = this.index.Mu[i];

                        for(int j = i1; j < i2; j++) {
                            int lj = this.index.L[j];
                            int muj = this.index.Mu[j];

                            // Selection rule
                            if(mui != muj && System.Math.Abs(mui - muj) != 1)
                                continue;

                            double sum = 0;

                            double[] vCache = vCache2;
                            if(mui == muj)
                                vCache = vCache1;

                            for(int sb = 0; sb < numSteps; sb++)
                                sum += cache1[i, sb] * vCache[sb] * cache2[j, sb];

                            sum *= step;

                            if(mui != muj) {
                                int mu = System.Math.Max(mui, muj);
                                sum *= mu / System.Math.Sqrt((2.0 * mu - 1.0) * (2.0 * mu + 1.0));
                            }

                            // Již je symetrické
                            m[i, j] = sum;
                        }
                    }

                    if(writer != null) {
                        writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));
                        startTime1 = DateTime.Now;
                    }
                }

                cache1 = cache2;
            }

            if(writer != null) {
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            return m;
        }

        /// <summary>
        /// Vrátí matici <n|V|n> vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        private Matrix EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = this.eigenVectors[n];

            Matrix result = new Matrix(intx.Num, inty.Num);

            DiscreteInterval intr = new DiscreteInterval(0,
                System.Math.Max(System.Math.Max(System.Math.Abs(intx.Min), System.Math.Abs(intx.Max)),
                    System.Math.Max(System.Math.Abs(inty.Min), System.Math.Abs(inty.Max))),
                intx.Num + inty.Num);

            int length = this.index.Length;

            BasisCache rcache = new BasisCache(intr, length, this.Psi);
            BasisCache acache = new BasisCache(new DiscreteInterval(0, 2.0 * System.Math.PI, intr.Num * 4), length, this.APsi);

            for(int i = 0; i < length; i++) {
                int li = this.index.L[i];
                int mui = this.index.Mu[i];

                for(int sx = 0; sx < intx.Num; sx++) {
                    double x = intx.GetX(sx);
                    for(int sy = 0; sy < inty.Num; sy++) {
                        double y = inty.GetX(sy);
                        double beta = System.Math.Sqrt(x * x + y * y);
                        double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));
                        result[sx, sy] += rcache.GetValue(i, beta) * ev[i] * acache.GetValue(i, gamma);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public override Matrix DensityMatrix(int n, params Vector[] interval) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            Matrix result = this.EigenMatrix(n, intx, inty);

            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] *= result[sx, sy];

            return result;
        }

        /// <summary>
        /// Radial part of the wave function
        /// </summary>
        /// <param name="l">Principal quantum number</param>
        /// <param name="mu">Second quantum number</param>
        /// <param name="x">Value</param>
        /// <param name="s">Multiplication factor</param>
        public double Psi(double x, int l, int mu) {
            double xi2 = this.s * x; xi2 *= xi2;
            int lambda = 3 * mu;

            double normLog = (lambda + 2.5) * System.Math.Log(this.s) + 0.5 * (SpecialFunctions.FactorialILog(l) - SpecialFunctions.HalfFactorialILog(lambda + l + 2) + System.Math.Log(2.0));
            double r = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out r, out e, xi2, l, lambda + 1.5);

            if(r == 0.0 || x == 0.0)
                return 0.0;

            double rLog = System.Math.Log(System.Math.Abs(r));
            double result = normLog + lambda * System.Math.Log(x) - xi2 / 2.0 + rLog + e;
            result = r < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="i">Index (kvantová èísla zjistíme podle uchované cache indexù)</param>
        /// <param name="x">Souøadnice</param>
        protected double Psi(double x, int i) {
            return Psi(x, this.index.L[i], this.index.Mu[i]);
        }

        /// <summary>
        /// Angular part of the wave function
        /// </summary>
        /// <param name="g">Angle gamma</param>
        /// <param name="i">Index</param>
        public double APsi(double g, int i) {
            int mu = this.index.Mu[i];
            double result = SpecialFunctions.Legendre(System.Math.Cos(3.0 * g), mu);
            double norm = System.Math.Sqrt((2.0 * mu + 1.0) / 4.0);
            return result * norm;
        }

        /// <summary>
        /// Vlnová funkce ve 2D
        /// </summary>
        protected override double PsiXY(double x, double y, int n) {
            double beta = System.Math.Sqrt(x * x + y * y);
            double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));

            return this.Psi(beta, n) * this.APsi(gamma, n);
        }

        #region Implementace IExportable
        protected override void Export(IEParam param) {
            if(this.isComputed)
                param.Add(this.index.MaxE, "Maximum Energy of Basis Functions");
        }

        protected override void Import(IEParam param) {
            if(this.isComputed)
                this.CreateIndex((int)param.Get(10));
        }

        public LHOQuantumGCMI5D(Core.Import import) : base(import) { }

        #endregion
    }
}