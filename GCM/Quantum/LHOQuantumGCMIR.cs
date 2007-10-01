using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// </summary>
    public class LHOQuantumGCMIR: LHOQuantumGCMI {
        // Indexy báze
        protected LHOPolarIndex index;
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LHOQuantumGCMIR() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMIR(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        protected override int MaximalNumNodes { get { return this.index.MaxM; } }
        protected override double MaximalRange { get { return System.Math.Sqrt(this.Hbar * this.Omega * this.index.MaxE / this.A0); } }
        protected override double PsiRange(double range) {
            return Psi(range, this.index.MaxN, this.index.MaxM);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        protected virtual void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE);
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
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="maxE">Nejvyšší energie v násobcích hbar * Omega</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            if(numSteps == 0)
                numSteps = 10 * this.index.MaxM + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("Maximální (n, m) = ({0}, {1})", this.index.MaxN, this.index.MaxM));
                writer.WriteLine(string.Format("Velikost báze: {0}", this.index.Length));
                writer.WriteLine(string.Format("Pøipravuji cache ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange();

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(new DiscreteInterval(0.0, range, numSteps), this.index.Length, this.Psi);

            int[] psiCacheLowerLimits = psiCache.GetLowerLimits(epsilon);
            int[] psiCacheUpperLimits = psiCache.GetUpperLimits(epsilon);

            double step = psiCache.Step;

            // Cache hodnot potencialu
            double[] vCache1 = new double[numSteps];
            double[] vCache2 = new double[numSteps];
            for(int sb = 0; sb < numSteps; sb++) {
                double beta = psiCache.GetX(sb);
                double beta2 = beta * beta;
                vCache1[sb] = beta2 * beta * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = 0.5 * this.B * beta2 * beta2;
            }

            int length = this.index.Length;
            Matrix m = new Matrix(length);

            if(writer != null)
                writer.WriteLine(string.Format("Pøíprava H ({0} x {1})", length, length));

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                for(int j = i; j < length; j++) {
                    int nj = this.index.N[j];
                    int mj = this.index.M[j];

                    // Výbìrové pravidlo
                    if(mi != mj && System.Math.Abs(mi - mj) != 3)
                        continue;

                    double sum = 0;

                    double[] vCache = vCache2;
                    if(mi == mj)
                        vCache = vCache1;

                    for(int sb = 0; sb < numSteps; sb++)
                        sum += psiCache[i, sb] * vCache[sb] * psiCache[j, sb];

                    sum *= step;

                    if(mi == mj && ni == nj)
                        sum += this.Hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                    m[i, j] = sum;
                    m[j, i] = sum;
                }

                // Výpis teèky na konzoli
                if(writer != null) {
                    if(i != 0 && this.index.M[i - 1] != this.index.M[i])
                        writer.Write(".");
                }
            }

            return m;
        }

        /// <summary>
        /// Vrátí indexy vlastních èísel, které mají sudou paritu
        /// </summary>
        public Vector Parity() {
            int length = this.index.Length;
            int num = this.eigenVectors.Length;

            Vector result = new Vector(num);

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                if(mi > 0)
                    continue;

                for(int j = i; j < length; j++) {
                    int nj = this.index.N[j];
                    int mj = this.index.M[j];

                    if(ni == nj && mi == -mj) {
                        for(int k = 0; k < num; k++)
                            result[k] += System.Math.Abs(this.eigenVectors[k][i] + this.eigenVectors[k][j]);
                        break;
                    }
                }
            }

            for(int k = 0; k < num; k++)
                if(result[k] > 1.0)
                    result[k] = 1.0;
                else
                    result[k] = -1.0;

            return result;
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
                numSteps = 10 * this.index.MaxM + 1;

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
                vCache1[sb] = beta2 * beta * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = 0.5 * this.B * beta2 * beta2;
            }

            int length = this.index.Length;
            int bandWidth = maxE - 2;
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

                // Diagonální blok
                for(int i = i0; i < i1; i++) {
                    int ni = this.index.N[i];
                    int mi = this.index.M[i];

                    for(int j = i; j < i1; j++) {
                        int nj = this.index.N[j];
                        int mj = this.index.M[j];

                        // Výbìrové pravidlo
                        if(mi != mj && System.Math.Abs(mi - mj) != 3)
                            continue;

                        // Only trace
                        if(trace && i != j)
                            continue;

                        double sum = 0;

                        double[] vCache = vCache2;
                        if(mi == mj)
                            vCache = vCache1;

                        for(int sb = 0; sb < numSteps; sb++)
                            sum += cache1[i, sb] * vCache[sb] * cache1[j, sb];

                        sum *= step;

                        if(mi == mj && ni == nj)
                            sum += this.Hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                        // Již je symetrické
                        m[i, j] = sum;
                    }
                }

                if(writer != null)
                    writer.Write('C');

                cache2 = new BasisCache(interval, i1, i2, this.Psi);

                if(!trace) {
                    if(writer != null)
                        writer.Write("N ");

                    // Nediagonální blok
                    for(int i = i0; i < i1; i++) {
                        int ni = this.index.N[i];
                        int mi = this.index.M[i];

                        for(int j = i1; j < i2; j++) {
                            int nj = this.index.N[j];
                            int mj = this.index.M[j];

                            // Výbìrové pravidlo
                            if(mi != mj && System.Math.Abs(mi - mj) != 3)
                                continue;

                            double sum = 0;

                            double[] vCache = vCache2;
                            if(mi == mj)
                                vCache = vCache1;

                            for(int sb = 0; sb < numSteps; sb++)
                                sum += cache1[i, sb] * vCache[sb] * cache2[j, sb];

                            sum *= step;

                            // Již je symetrické
                            m[i, j] = sum;
                        }
                    }
                }

                cache1 = cache2;

                if(writer != null) {
                    writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));
                    startTime1 = DateTime.Now;
                }
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
        private Complex[,] EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = this.eigenVectors[n];

            Complex[,] result = new Complex[intx.Num, inty.Num];
            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = new Complex();

            DiscreteInterval intr = new DiscreteInterval(0,
                System.Math.Max(System.Math.Max(System.Math.Abs(intx.Min), System.Math.Abs(intx.Max)),
                    System.Math.Max(System.Math.Abs(inty.Min), System.Math.Abs(inty.Max))),
                intx.Num + inty.Num);

            int length = this.index.Length;

            BasisCache cache = new BasisCache(intr, length, this.Psi);

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                for(int sx = 0; sx < intx.Num; sx++) {
                    double x = intx.GetX(sx);
                    for(int sy = 0; sy < inty.Num; sy++) {
                        double y = inty.GetX(sy);
                        double beta = System.Math.Sqrt(x * x + y * y);
                        double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));
                        result[sx, sy] += cache.GetValue(i, beta) * ev[i] * Complex.Exp(Complex.I * mi * gamma);
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

            Complex[,] m = this.EigenMatrix(n, intx, inty);
            Matrix result = new Matrix(m.GetLength(0), m.GetLength(1));

            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = m[sx, sy].SquaredNorm;

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="m">Spin</param>
        /// <param name="x">Souøadnice</param>
        public double Psi(double x, int n, int m) {
            double xi2 = this.s * x; xi2 *= xi2;
            m = System.Math.Abs(m);

            double normLog = 0.5 * (System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n) - SpecialFunctions.FactorialILog(n + m)) + (m + 1) * System.Math.Log(this.s);
            double l = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out l, out e, xi2, n, m);

            if(l == 0.0 || x == 0.0)
                return 0.0;

            double lLog = System.Math.Log(System.Math.Abs(l));
            double result = normLog + m * System.Math.Log(x) - xi2 / 2.0 + lLog + e;
            result = l < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="i">Index (kvantová èísla zjistíme podle uchované cache indexù)</param>
        /// <param name="x">Souøadnice</param>
        protected double Psi(double x, int i) {
            return this.Psi(x, this.index.N[i], this.index.M[i]);
        }

        /// <summary>
        /// Vrátí èasovou støední hodnotu druhého integrálu - úhlového momentu -i * hbar * (d / d phi)
        /// </summary>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector GetSecondInvariant() {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenVectors[i];
                int length = ev.Length;

                for(int j = 0; j < length; j++)
                    result[i] += ev[j] * ev[j] * System.Math.Abs(this.index.M[j]);

                result[i] *= this.Hbar;
            }

            return result;
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

        public LHOQuantumGCMIR(Core.Import import) : base(import) { }
        #endregion
    }
}