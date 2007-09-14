using System;
using System.Collections;
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
    public class LHOQuantumGCM5D1: LHOQuantumGCM {
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
        public LHOQuantumGCM5D1(double a, double b, double c, double k, double a0)
            : base(a, b, c, k, a0) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCM5D1(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
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
        /// Vypoèítá Hamiltonovu matici
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(maxE, numSteps, writer);
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici do tvaru pásové matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        protected SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return this.HamiltonianSBMatrix(maxE, numSteps, writer, false);
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici do tvaru pásové matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="writer">Writer</param>
        /// <param name="trace">Calculates only trace of the matrix</param>
        protected virtual SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer, bool trace) {
            this.CreateIndex(maxE);

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine("Hamiltonova matice");
                writer.Indent(1);
            }

            double omega = this.Omega;

            int length = this.index.Length;
            int bandWidth = trace ? 0 : maxE;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            if(writer != null)
                writer.WriteLine(string.Format("Pøíprava pásové matice H ({0} x {1})", length, maxE - 2));

            DateTime startTime1 = DateTime.Now;

            for(int i = 0; i < length; i++) {
                int li = this.index.L[i];
                int mui = this.index.Mu[i];

                for(int j = 0; j < length; j++) {
                    int lj = this.index.L[j];
                    int muj = this.index.Mu[j];

                    // Selection rule
                    if(mui != muj && System.Math.Abs(mui - muj) != 1)
                        continue;

                    // If only trace
                    if(trace && i != j)
                        continue;

                    double sum = 0;
/*
                    if(mui == muj)
                        vCache = vCache1;

                    for(int sb = 0; sb < numSteps; sb++)
                        sum += cache1[i, sb] * vCache[sb] * cache1[j, sb];

                    sum *= step;

                    if(mui != muj) {
                        int mu = System.Math.Max(mui, muj);
                        sum *= mu / System.Math.Sqrt((2.0 * mu - 1.0) * (2.0 * mu + 1.0));
                    }
                    */
                    if(mui == muj && li == lj)
                        sum += this.Hbar * omega * (2.5 + li + li + 3 * mui);

                    // Již je symetrické
                    m[i, j] = sum;
                }
            }


            if(writer != null) {
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            return m;
        }

        public static double L2Integral(int l1, int mu1, int l2, int mu2, int p) {
            int lambda1 = 3*mu1;
            int lambda2 = 3*mu2;

            int s = lambda1 + lambda2 + p + 3;
            bool shalf = s % 2 != 0;

            LongFraction sum = new LongFraction(0);
            ArrayList a = new ArrayList();

            for(int i = 0; i <= l1; i++)
                for(int j = 0; j <= l2; j++) {
                    LongFraction l = SpecialFunctions.HalfFactorialL(l1 + lambda1 + 2) / SpecialFunctions.FactorialL(l1 - i) / SpecialFunctions.HalfFactorialL(lambda1 + 2 + i) / SpecialFunctions.FactorialL(i);
                    l *= SpecialFunctions.HalfFactorialL(l2 + lambda2 + 2) / SpecialFunctions.FactorialL(l2 - j) / SpecialFunctions.HalfFactorialL(lambda2 + 2 + j) / SpecialFunctions.FactorialL(j);
                    l *= shalf ? SpecialFunctions.HalfFactorialL(s / 2 + i + j + 1) : SpecialFunctions.FactorialL(s / 2 + i + j);
                    l.Abbreviate();
                    sum += l;
                }

            return (double)sum;
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override void Compute(int maxE, int numSteps, bool ev, int numev, IOutputWriter writer) {
            if(this.isComputing)
                throw new GCMException(errorMessageComputing);
            this.isComputing = true;

            if(numev <= 0 || numev > this.HamiltonianMatrixSize(maxE))
                numev = this.HamiltonianMatrixSize(maxE);

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine(string.Format("{0} ({1}): Výpoèet {2} vlastních hodnot{3}.",
                    this.GetType().Name,
                    startTime,
                    numev,
                    ev ? " a vektorù" : string.Empty));
                writer.Indent(1);
                writer.WriteLine(string.Format("Maximální (l, mu) = ({0}, {1})", this.index.MaxL, this.index.MaxMu));
                writer.WriteLine(string.Format("Velikost báze: {0}", this.index.Length));
            }

            SymmetricBandMatrix m = this.HamiltonianSBMatrix(maxE, numSteps, writer);

            if(writer != null) {
                writer.WriteLine(string.Format("Stopa matice: {0}", m.Trace()));
                writer.WriteLine("Èištìní pamìti...");
            }

            // Musíme uvolnit co nejvíce pamìti
            GC.Collect();

            if(writer != null)
                writer.Write("Diagonalizace dsbevx... ");

            DateTime startTime1 = DateTime.Now;

            Vector[] eigenSystem = LAPackDLL.dsbevx(m, ev, 0, numev);
            m.Dispose();

            if(writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));

            this.eigenValues = eigenSystem[0];
            this.eigenValues.Length = numev;

            if(ev) {
                this.eigenVectors = new Vector[numev];
                for(int i = 0; i < numev; i++)
                    this.eigenVectors[i] = eigenSystem[i + 1];
            }
            else
                this.eigenVectors = new Vector[0];

            if(writer != null) {
                writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", this.eigenValues.Sum()));
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            this.isComputed = true;
            this.isComputing = false;
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
        /// Zkontroluje, zda je range úplný a pøípadnì doplní
        /// </summary>
        /// <param name="range">Vstupní rozmìry</param>
        /// <returns>Výstupní rozmìry</returns>
        private DiscreteInterval ParseRange(Vector range) {
            if((object)range == null || range.Length == 0)
                return new DiscreteInterval(this.GetRange(epsilon), 10 * this.index.MaxMu + 1);

            if(range.Length == 1)
                return new DiscreteInterval(this.GetRange(epsilon), (int)range[0]);

            if(range.Length == 2)
                return new DiscreteInterval(range[0], (int)range[1]);

            return new DiscreteInterval(range);
        }

        /// <summary>
        /// Vrací parametr range podle dosahu nejvyšší použité vlastní funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maximální rank vlastní funkce</param>
        protected double GetRange(double epsilon) {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = System.Math.Sqrt(this.Hbar * this.Omega * this.index.MaxE / this.A0);
            range *= 5.0;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * this.index.MaxMu);

            while(System.Math.Abs(Psi(range, this.index.MaxL, this.index.MaxMu, this.s)) < epsilon)
                range -= dx;

            //jedno dx, abysme se dostali tam, co to bylo male a druhe jako rezerva
            return range + 2 * dx;
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public override Matrix DensityMatrix(int n, params Vector[] interval) {
            if(!this.isComputed)
                throw new GCMException(errorMessageNotComputed);

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
        public static double Psi(double x, int l, int mu, double s) {
            double xi2 = s * x; xi2 *= xi2;
            int lambda = 3 * mu;

            double normLog = (lambda + 2.5) * System.Math.Log(s) + 0.5 * (SpecialFunctions.FactorialILog(l) - SpecialFunctions.HalfFactorialILog(lambda + l + 2) + System.Math.Log(2.0));
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
            return Psi(x, this.index.L[i], this.index.Mu[i], this.s);
        }

        /// <summary>
        /// Angular part of the wave function
        /// </summary>
        /// <param name="g">Angle gamma</param>
        /// <param name="i">Index</param>
        protected double APsi(double g, int i) {
            int mu = this.index.Mu[i];
            double result = SpecialFunctions.Legendre(System.Math.Cos(3.0 * g), mu);
            double norm = System.Math.Sqrt((2.0 * mu + 1.0) / 4.0);
            return result * norm;
        }

        protected override void Export(IEParam param) {
            if(this.isComputed)
                param.Add(this.index.MaxE, "Maximum Energy of Basis Functions");
        }

        protected override void Import(IEParam param) {
            if(this.isComputed)
                this.CreateIndex((int)param.Get(10));
        }
    }
}