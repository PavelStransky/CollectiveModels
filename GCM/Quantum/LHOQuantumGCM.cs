using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// </summary>
    public class LHOQuantumGCM : GCM, IExportable, IQuantumSystem {
        private const double epsilon = 1E-8;
        private double hbar;                    // [Js]

        // Koeficienty
        private double s, n;

        // Parametr pro LHO
        private double a0;

        private HermitPolynom hermit;
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
        public LHOQuantumGCM() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCM(double a, double b, double c, double k, double a0)
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
        public LHOQuantumGCM(double a, double b, double c, double k, double a0, double hbar)
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
            this.n = System.Math.Sqrt(System.Math.Sqrt(this.K * this.Omega / (System.Math.PI * this.hbar)));
                                                        // sqrt(sqrt(M*omega/(pi*hbar))) (Formanek (2.286))
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="maxn">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Wirter</param>
        public void Compute(int maxn, int numSteps, IOutputWriter writer) {
            if(numSteps == 0)
                numSteps = 10 * maxn + 1;

            if(writer != null)
                writer.WriteLine(string.Format("Pøipravuji cache ({0} x {1})...", numSteps, numSteps));

            // Hermitùv polynom
            this.hermit = new HermitPolynom(maxn);
            double omega = this.Omega;
            double range = this.GetRange(maxn, epsilon);

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(new DiscreteInterval(range, numSteps), maxn, this.Psi);
            int[] psiCacheLowerLimits = psiCache.GetLowerLimits(epsilon);
            int[] psiCacheUpperLimits = psiCache.GetUpperLimits(epsilon);

            double step = psiCache.Step;

            // Cache hodnot potencialu
            double[,] vCache = new double[numSteps, numSteps];
            for(int sx = 0; sx < numSteps; sx++) {
                double x = psiCache.GetX(sx);
                for(int sy = 0; sy < numSteps; sy++) {
                    double y = psiCache.GetX(sy);
                    // Originální potenciál - potenciál báze
                    vCache[sx, sy] = this.V(x, y) - this.a0 * (x * x + y * y);
                }
            }

            int max2 = maxn * maxn;
            Matrix m = new Matrix(max2, max2);

            if(writer != null)
                writer.WriteLine(string.Format("Pøíprava H ({0} x {1})", max2, max2));

            DateTime startTime = DateTime.Now;
            DateTime startTime1 = startTime;

            for(int i = 0; i < max2; i++) {
                for(int j = i; j < max2; j++) {
                    int ix = i / maxn;
                    int iy = i % maxn;
                    int jx = j / maxn;
                    int jy = j % maxn;

                    double sum = 0;

                    int minsx = System.Math.Max(psiCacheLowerLimits[ix], psiCacheLowerLimits[jx]);
                    int maxsx = System.Math.Min(psiCacheUpperLimits[ix], psiCacheUpperLimits[jx]);

                    int minsy = System.Math.Max(psiCacheLowerLimits[iy], psiCacheLowerLimits[jy]);
                    int maxsy = System.Math.Min(psiCacheUpperLimits[iy], psiCacheUpperLimits[jy]);

                    for(int sy = minsy; sy < maxsy; sy++)
                        for(int sx = minsx; sx < maxsx; sx++)
                            sum += psiCache[ix, sx] * psiCache[iy, sy] * vCache[sx, sy] * 
                                psiCache[jx, sx] * psiCache[jy, sy];

                    sum *= step * step;

                    if(ix == jx && iy == jy)
                       sum += this.hbar * omega * (1.0 + ix + iy);

                    m[i, j] = sum;
                    m[j, i] = sum;
                }

                // Výpis teèky na konzoli
                if(writer != null) {
                    if(i % maxn == 0) {
                        if(i != 0)
                            writer.WriteLine((DateTime.Now - startTime1).ToString());

                        writer.Write(i / maxn);
                        startTime1 = DateTime.Now;
                    }

                    writer.Write(".");
                }
            }

            if(writer != null) {
                writer.WriteLine((DateTime.Now - startTime).ToString());
                writer.WriteLine(string.Format("Stopa matice: {0}", m.Trace()));
            }

            startTime = DateTime.Now;

            this.jacobi = new Jacobi(m, writer);
            this.jacobi.SortAsc();

            if(writer != null) {
                writer.WriteLine((DateTime.Now - startTime).ToString());
                writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", new Vector(this.jacobi.EigenValue).Sum()));
            }
        }

        /// <summary>
        /// Nejvyšší použitý øád Hermitova polynomu
        /// </summary>
        public int MaxN { get { return this.hermit.MaxN; } }

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
        private Matrix EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = jacobi.EigenVector[n];
            Matrix result = new Matrix(intx.Num, inty.Num);

            int sqrlength = ev.Length;
            int length = (int)System.Math.Round(System.Math.Sqrt(sqrlength));

            BasisCache cachex = new BasisCache(intx, this.MaxN, this.Psi);
            BasisCache cachey = new BasisCache(inty, this.MaxN, this.Psi);

            for(int i = 0; i < sqrlength; i++) {
                int ix = i / length;
                int iy = i % length;

                for(int sx = 0; sx < intx.Num; sx++) 
                    for(int sy = 0; sy < inty.Num; sy++) 
                        result[sx, sy] += cachex[ix, sx] * cachey[iy, sy] * ev[i];
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
                return new DiscreteInterval(this.GetRange(this.MaxN, epsilon), 10 * this.MaxN + 1);

            if(range.Length == 1)
                return new DiscreteInterval(this.GetRange(this.MaxN, epsilon), (int)range[0]);

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

            Matrix result = this.EigenMatrix(n, intx, inty);
            
            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = result[sx, sy] * result[sx, sy];

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
        /// Diference |H|psi> - E|psi>|^2 pro zadanou vlastní funkci
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public Matrix NumericalDiff(int n, params Vector[] interval) {
            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            Matrix em = this.EigenMatrix(n, intx, inty);
            Matrix result = new Matrix(intx.Num, inty.Num);

            for(int sx = 1; sx < intx.Num - 1; sx++) {
                double x = intx.GetX(sx);
                for(int sy = 1; sy < inty.Num - 1; sy++) {
                    double y = inty.GetX(sy);

                    double laplace = (em[sx + 1, sy] + em[sx - 1, sy] + em[sx, sy - 1] + em[sx, sy + 1]
                        - 4.0 * em[sx, sy]) / (intx.Step * inty.Step);

                    //laplace = 1 / (dx * dx) * (
                    //    -(M[i + 2, j] + M[i - 2, j] + M[i, j + 2] + M[i, j - 2]) / 12 +
                    //    4 * (M[i + 1, j] + M[i - 1, j] + M[i, j + 1] + M[i, j - 1]) / 3 - 5 * M[i, j]
                    //    ); //O(dx^4)

                    result[sx, sy] = -this.hbar * this.hbar / (2 * this.K) * laplace + em[sx, sy] * this.V(x, y) - em[sx, sy] * this.jacobi.EigenValue[n];
                    result[sx, sy] *= result[sx, sy];
                }
            }

            return result;
        }

        /// <summary>
        /// Funkce Psi_n (Formánek 2.286)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Psi(int n, double x) {
            double xi = this.s * x;
            return this.n / System.Math.Sqrt(SpecialFunctions.Factorial(n) * System.Math.Pow(2, n)) * hermit.GetValue(n, xi) * System.Math.Exp(-xi * xi / 2);
        }

        /// <summary>
        /// Vrací parametr range podle dosahu nejvyšší použité vlastní funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maximální rank vlastní funkce</param>
        private double GetRange(int maxn, double epsilon) {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = System.Math.Sqrt(hbar * this.Omega * (maxn + 0.5) / this.a0);
            range *= 5.0;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * maxn);
            
            while(System.Math.Abs(this.Psi(maxn, range)) < epsilon)
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
            this.jacobi = (Jacobi)param.Get();

            this.hermit = new HermitPolynom((int)System.Math.Sqrt(this.jacobi.EigenValue.Length));
            this.RefreshConstants();
        }
        #endregion
    }
}