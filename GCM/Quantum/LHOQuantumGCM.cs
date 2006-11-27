using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// </summary>
    public class LHOQuantumGCM : GCM, IExportable, IQuantumSystem {
        private const double epsilon = 1E-10;
        private double hbar = 1.0E-1;	// [Js]

        // Koeficienty
        private double s, n;

        // Parametr pro LHO
        private double a0;

        private HermitPolynom hermit;
        private Jacobi jacobi;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } set { this.hbar = value; } }

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
        /// <param name="lambda">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCM(double lambda, double a, double b, double c, double k)
            : base((1 - lambda) * a, b, c, k) {
            this.a0 = lambda * a;

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
            double step = 2.0 * range / numSteps;

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(-range, step, numSteps, maxn, this.Psi);
            int[] psiCacheLimits = this.GetPsiCacheLimits(psiCache, epsilon);

            // Cache hodnot potencialu
            double[,] vCache = new double[numSteps, numSteps];
            for(int sy = 0; sy < numSteps; sy++)
                for(int sx = 0; sx < numSteps; sx++)
                    vCache[sx, sy] = this.V(-range + step * sx, -range + step * sy);

            int max2 = maxn * maxn;
            Matrix m = new Matrix(max2, max2);

            if(writer != null)
                writer.WriteLine(string.Format("Pøíprava H ({0} x {1})", max2, max2));

            for(int i = 0; i < max2; i++) {
                for(int j = i; j < max2; j++) {
                    int ix = i / maxn;
                    int iy = i % maxn;
                    int jx = j / maxn;
                    int jy = j % maxn;

                    double sum = 0;

                    int minsx = System.Math.Max(psiCacheLimits[ix], psiCacheLimits[jx]);
                    int maxsx = numSteps - minsx;

                    int minsy = System.Math.Max(psiCacheLimits[iy], psiCacheLimits[jy]);
                    int maxsy = numSteps - minsy;

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

                // Výpis na konzoli
                if(writer != null) {
                    if(i % maxn == 0 && i != 0)
                        writer.WriteLine();

                    writer.Write(".");
                }
            }

            if(writer != null)
                writer.WriteLine();

            DateTime startTime = DateTime.Now;
            this.jacobi = new Jacobi(m, writer);
            this.jacobi.SortAsc();

            if(writer != null)
                writer.WriteLine((DateTime.Now - startTime).ToString());
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
        /// <param name="range">Rozmìry v jednotlivých rozmìrech (uspoøádané ve tvaru minx, maxx, numx, ...)</param>
        public Matrix EigenMatrix(int n, Vector range) {
            if(range.Length != 6)
                throw new GCMException("Pro správné vytvoøení matice hustot vlastních vektorù je nutné, aby vstupní vektor mìl 6 prvkù!");

            double minx = range[0]; double maxx = range[1];
            int numx = (int)range[2];
            double koefx = (maxx - minx) / (numx - 1);

            double miny = range[3]; double maxy = range[4];
            int numy = (int)range[5];
            double koefy = (maxy - miny) / (numy - 1);

            Vector ev = jacobi.EigenVector[n];
            Matrix result = new Matrix(numx, numy);

            int length = ev.Length;
            int sqrtlength = (int)System.Math.Round(System.Math.Sqrt(length));

            for(int i = 0; i < length; i++) {
                int ix = i / sqrtlength;
                int iy = i % sqrtlength;

                for(int sx = 0; sx < numx; sx++) {
                    double x = minx + koefx * sx;
                    for(int sy = 0; sy < numy; sy++) {
                        double y = miny + koefy * sy;
                        result[sx, sy] += this.Psi(ix, x) * this.Psi(iy, y) * ev[i];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="range">Rozmìry v jednotlivých rozmìrech (uspoøádané ve tvaru minx, maxx, numx, ...)</param>
        public Matrix DensityMatrix(int n, Vector range) {
            Matrix result = this.EigenMatrix(n, range);
            
            int numx = result.LengthX;
            int numy = result.LengthY;

            for(int sx = 0; sx < numx; sx++)
                for(int sy = 0; sy < numy; sy++)
                    result[sx, sy] = result[sx, sy] * result[sx, sy];

            // Normování
            result = result * (1.0 / System.Math.Abs(result.MaxAbs()));

            // Zakreslení ekvipotenciální kontury
            double minx = range[0]; double maxx = range[1];
            double koefx = (maxx - minx) / (numx - 1);

            double miny = range[3]; double maxy = range[4];
            double koefy = (maxy - miny) / (numy - 1);

            PointVector[] pv = this.EquipotentialContours(this.EigenValue[n]);
            for(int i = 0; i < pv.Length; i++) {
                int pvlength = pv[i].Length;
                for(int j = 0; j < pvlength; j++) {
                    int sx = (int)((pv[i][j].X - minx) / koefx);
                    int sy = (int)((pv[i][j].Y - miny) / koefy);

                    if(sx >= 0 && sx < numx && sy >= 0 && sy < numy)
                        result[sx, sy] = -1.0;
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

        /// <summary>
        /// Vrací první indexy psiCache, které jsou vìtší než epsilon
        /// </summary>
        /// <param name="psiCache">psiCache</param>
        /// <param name="epsilon">Epsilon</param>
        private int[] GetPsiCacheLimits(BasisCache psiCache, double epsilon) {
            int[] result = new int[psiCache.MaxN];

            for(int i = 0; i < psiCache.MaxN; i++) {
                int limit = 0;
                while(limit < psiCache.MaxIndex && System.Math.Abs(psiCache[i, limit]) < epsilon)
                    limit++;
                result[i] = System.Math.Max(0, limit - 1);
            }

            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;
                b.Write(this.A);
                b.Write(this.B);
                b.Write(this.C);
                b.Write(this.K);
                b.Write(this.A0);
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", this.A, this.B, this.C, this.K, this.A0);
            }

            export.Write(this.jacobi);
		}

		/// <summary>
		/// Naète výsledky ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                this.A = b.ReadDouble();
                this.B = b.ReadDouble();
                this.C = b.ReadDouble();
                this.K = b.ReadDouble();
                this.a0 = b.ReadDouble();
            }
            else {
                // Textovì
                StreamReader t = import.T;
                string line = t.ReadLine();
                string[] s = line.Split('\t');
                this.A = double.Parse(s[0]);
                this.B = double.Parse(s[1]);
                this.C = double.Parse(s[2]);
                this.K = double.Parse(s[3]);
                this.A0 = double.Parse(s[4]);
            }

            this.jacobi = import.Read() as Jacobi;
            this.hermit = new HermitPolynom(this.jacobi.EigenValue.Length);
            this.RefreshConstants();
        }
        #endregion
    }
}