using System;
using System.IO;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// </summary>
    public class LHOQuantumGCMRFull : GCM, IExportable, IQuantumSystem {
        private const double epsilon = 1E-8;
        private double hbar;                    // [Js]

        // Koeficienty
        private double s;

        // Parametr pro LHO
        private double a0;

        // Indexy báze
        private LHOPolarIndexFull index;

        private Jacobi jacobi;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// Úhlová frekvence LHO [J*m^-2]
        /// </summary>
        public double Omega { get { return System.Math.Sqrt(2.0 * this.a0 / this.K); } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public LHOQuantumGCMRFull() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCMRFull(double a, double b, double c, double k, double a0)
            : this(a, b, c, k, a0, 0.1) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMRFull(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k) {
            this.a0 = a0;
            this.hbar = hbar;

            this.RefreshConstants();
        }

        /// <summary>
        /// Pøepoèítá konstanty s, n
        /// </summary>
        private void RefreshConstants() {
            // Konstanty
            this.s = System.Math.Sqrt(this.K * this.Omega / this.hbar);      // xi = s*x (Formanek (2.283))
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="maxE">Nejvyšší energie v násobcích hbar * Omega</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Wirter</param>
        public void Compute(int maxE, int numSteps, IOutputWriter writer) {
            this.index = new LHOPolarIndexFull(maxE);

            if(numSteps == 0)
                numSteps = 10 * this.index.MaxM + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("Maximální (n, m) = ({0}, {1})", this.index.MaxN, this.index.MaxM));
                writer.WriteLine(string.Format("Velikost báze: {0}", this.index.Length));
                writer.WriteLine(string.Format("Pøipravuji cache ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange(epsilon);

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

            DateTime startTime = DateTime.Now;

            for(int i = 0; i < length; i++) {
                for(int j = i; j < length; j++) {
                    int ni = this.index.N[i];
                    int mi = this.index.M[i];
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
                        sum += this.hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                    m[i, j] = sum;
                    m[j, i] = sum;
                }

                // Výpis teèky na konzoli
                if(writer != null) {
                    if(i != 0 && this.index.N[i - 1] != this.index.N[i])
                        writer.Write(".");
                }
            }

            if(writer != null) {
                writer.WriteLine((DateTime.Now - startTime).ToString());
                writer.WriteLine(string.Format("Stopa matice: {0}", m.Trace()));
            }

            this.jacobi = new Jacobi(m, writer);
            this.jacobi.SortAsc();

            if(writer != null) {
                writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", new Vector(this.jacobi.EigenValue).Sum()));
            }
        }

        /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        public double[] EigenValue { get { return this.jacobi.EigenValue; } }

        /// <summary>
        /// Vlastní vektory
        /// </summary>
        public Vector[] EigenVector { get { return this.jacobi.EigenVector; } }

        /// <summary>
        /// Vrátí matici <n|V|n> vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        private Complex[,] EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = jacobi.EigenVector[n];

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
        /// Zkontroluje, zda je range úplný a pøípadnì doplní
        /// </summary>
        /// <param name="range">Vstupní rozmìry</param>
        /// <returns>Výstupní rozmìry</returns>
        private DiscreteInterval ParseRange(Vector range) {
            if((object)range == null || range.Length == 0)
                return new DiscreteInterval(this.GetRange(epsilon), 10 * this.index.MaxM + 1);

            if(range.Length == 1)
                return new DiscreteInterval(this.GetRange(epsilon), (int)range[0]);

            if(range.Length == 2)
                return new DiscreteInterval(range[0], (int)range[1]);

            return new DiscreteInterval(range);
        }
        
        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public Matrix DensityMatrix(int n, params Vector[] interval) {
            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            Complex[,] m = this.EigenMatrix(n, intx, inty);
            Matrix result = new Matrix(m.GetLength(0), m.GetLength(1));

            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = m[sx, sy].SquaredNorm;

            // Zakreslení ekvipotenciální kontury
            if(interval.Length > 2) {
                double maxAbs = System.Math.Abs(result.MaxAbs());

                PointVector[] pv = this.EquipotentialContours(this.EigenValue[n]);
                for(int i = 0; i < pv.Length; i++) {
                    int pvlength = pv[i].Length;
                    for(int j = 0; j < pvlength; j++) {
                        int sx = intx.GetIndex(pv[i][j].X);
                        int sy = inty.GetIndex(pv[i][j].Y);

                        if(sx >= 0 && sx < intx.Num && sy >= 0 && sy < inty.Num)
                            result[sx, sy] = -maxAbs;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="m">Spin</param>
        /// <param name="x">Souøadnice</param>
        private double Psi(int n, int m, double x) {
            double xi2 = this.s * x; xi2 *= xi2;
            m = System.Math.Abs(m);
            double norm = System.Math.Sqrt(2.0 * SpecialFunctions.FactorialI(n) / SpecialFunctions.FactorialI(n + m)) * System.Math.Pow(this.s, m + 1);
            return norm * System.Math.Pow(x, m) * System.Math.Exp(-xi2 / 2) * SpecialFunctions.Laguerre(n, m, xi2);
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="i">Index (kvantová èísla zjistíme podle uchované cache indexù)</param>
        /// <param name="x">Souøadnice</param>
        private double Psi(int i, double x) {
            return this.Psi(this.index.N[i], this.index.M[i], x);
        }

        /// <summary>
        /// Vrací parametr range podle dosahu nejvyšší použité vlastní funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maximální rank vlastní funkce</param>
        private double GetRange(double epsilon) {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = System.Math.Sqrt(hbar * this.Omega * this.index.MaxE / this.a0);
            range *= 5.0;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * this.index.MaxM);

            while(System.Math.Abs(this.Psi(this.index.MaxN, this.index.MaxM, range)) < epsilon)
                range -= dx;

            //jedno dx, abysme se dostali tam, co to bylo male a druhe jako rezerva
            return range + 2 * dx;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.A, "A");
            param.Add(this.B, "B");
            param.Add(this.C, "C");
            param.Add(this.K, "K");
            param.Add(this.A0, "A0");
            param.Add(this.Hbar, "HBar");
            param.Add(this.index.MaxE, "MaxE");
            param.Add(this.jacobi, "Jacobi");

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            IEParam param = new IEParam(import);

            this.A = (double)param.Get(-1.0);
            this.B = (double)param.Get(1.0);
            this.C = (double)param.Get(1.0);
            this.K = (double)param.Get(1.0);
            this.A0 = (double)param.Get(1.0);
            this.hbar = (double)param.Get(0.1);
            int maxE = (int)param.Get();
            this.jacobi = (Jacobi)param.Get();

            this.index = new LHOPolarIndexFull(maxE);
            this.RefreshConstants();
        }
        #endregion
    }
}