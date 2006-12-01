using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
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
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// �hlov� frekvence LHO [J*m^-2]
        /// </summary>
        public double Omega { get { return System.Math.Sqrt(2.0 * this.a0 / this.K); } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// Pr�zdn� konstruktor
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
            : base(a, b, c, k) {
            this.a0 = a0;

            this.RefreshConstants();
        }

        /// <summary>
        /// P�epo��t� konstanty s, n
        /// </summary>
        private void RefreshConstants() {
            // Konstanty
            this.s = System.Math.Sqrt(this.K * this.Omega / this.hbar);      // xi = s*x (Formanek (2.283))
            this.n = System.Math.Sqrt(System.Math.Sqrt(this.K * this.Omega / (System.Math.PI * this.hbar)));
                                                        // sqrt(sqrt(M*omega/(pi*hbar))) (Formanek (2.286))
        }

        /// <summary>
        /// Provede v�po�et
        /// </summary>
        /// <param name="maxn">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Wirter</param>
        public void Compute(int maxn, int numSteps, IOutputWriter writer) {
            if(numSteps == 0)
                numSteps = 10 * maxn + 1;

            if(writer != null)
                writer.WriteLine(string.Format("P�ipravuji cache ({0} x {1})...", numSteps, numSteps));

            // Hermit�v polynom
            this.hermit = new HermitPolynom(maxn);
            double omega = this.Omega;
            double range = this.GetRange(maxn, epsilon);

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(-range, range, numSteps, maxn, this.Psi);
            int[] psiCacheLowerLimits = psiCache.GetLowerLimits(epsilon);
            int[] psiCacheUpperLimits = psiCache.GetUpperLimits(epsilon);

            double step = psiCache.StepX;

            // Cache hodnot potencialu
            double[,] vCache = new double[numSteps, numSteps];
            for(int sx = 0; sx < numSteps; sx++) {
                double x = -range + step * sx;
                for(int sy = 0; sy < numSteps; sy++) {
                    double y = -range + step * sy;
                    // Origin�ln� potenci�l - potenci�l b�ze
                    vCache[sx, sy] = this.V(x, y) - this.a0 * (x * x + y * y);
                }
            }

            int max2 = maxn * maxn;
            Matrix m = new Matrix(max2, max2);

            if(writer != null)
                writer.WriteLine(string.Format("P��prava H ({0} x {1})", max2, max2));

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

                // V�pis te�ky na konzoli
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
        /// Nejvy��� pou�it� ��d Hermitova polynomu
        /// </summary>
        public int MaxN { get { return this.hermit.MaxN; } }

        /// <summary>
        /// Vlastn� hodnoty
        /// </summary>
        public double[] EigenValue { get { return this.jacobi.EigenValue; } }

        /// <summary>
        /// Vlastn� vektory
        /// </summary>
        public Vector[] EigenVector { get { return this.jacobi.EigenVector; } }

        /// <summary>
        /// Vr�t� matici <n|V|n> vlastn� funkce n
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="rx">Rozm�ry ve sm�ru x</param>
        /// <param name="ry">Rozm�ry ve sm�ru y</param>
        private Matrix EigenMatrix(int n, Range rx, Range ry) {
            Vector ev = jacobi.EigenVector[n];
            Matrix result = new Matrix(rx.Num, ry.Num);

            int sqrlength = ev.Length;
            int length = (int)System.Math.Round(System.Math.Sqrt(sqrlength));

            BasisCache cachex = new BasisCache(rx, this.MaxN, this.Psi);
            BasisCache cachey = new BasisCache(ry, this.MaxN, this.Psi);

            for(int i = 0; i < sqrlength; i++) {
                int ix = i / length;
                int iy = i % length;

                for(int sx = 0; sx < rx.Num; sx++) 
                    for(int sy = 0; sy < rx.Num; sy++) 
                        result[sx, sy] += cachex[ix, sx] * cachey[iy, sy] * ev[i];
            }

            return result;
        }

        /// <summary>
        /// Zkontroluje, zda je range �pln� a p��padn� dopln�
        /// </summary>
        /// <param name="range">Vstupn� rozm�ry</param>
        /// <returns>V�stupn� rozm�ry</returns>
        private Range ParseRange(Vector range) {
            if(range == null)
                return new Range(this.GetRange(this.MaxN, epsilon), this.MaxN);

            if(range.Length == 1)
                return new Range(this.GetRange(this.MaxN, epsilon), (int)range[0]);

            if(range.Length == 2)
                return new Range(range[0], (int)range[1]);

            return new Range(range);
        }

        /// <summary>
        /// Vr�t� matici hustot pro vlastn� funkce
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="range">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
        public Matrix DensityMatrix(int n, params Vector[] range) {
            Range rx = this.ParseRange(range.Length > 0 ? range[0] : null);
            Range ry = this.ParseRange(range.Length > 1 ? range[1] : null);

            Matrix result = this.EigenMatrix(n, rx, ry);
            
            for(int sx = 0; sx < rx.Num; sx++)
                for(int sy = 0; sy < ry.Num; sy++)
                    result[sx, sy] = result[sx, sy] * result[sx, sy];

            // Zakreslen� ekvipotenci�ln� kontury
            if(range.Length > 2) {
                // Normov�n�
                result = result * (1.0 / System.Math.Abs(result.MaxAbs()));

                PointVector[] pv = this.EquipotentialContours(this.EigenValue[n]);
                for(int i = 0; i < pv.Length; i++) {
                    int pvlength = pv[i].Length;
                    for(int j = 0; j < pvlength; j++) {
                        int sx = rx.GetIndex(pv[i][j].X);
                        int sy = ry.GetIndex(pv[i][j].Y);

                        if(sx >= 0 && sx < vr.NumX && sy >= 0 && sy < vr.NumY)
                            result[sx, sy] = -1.0;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Diference |H|psi> - E|psi>|^2 pro zadanou vlastn� funkci
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="range">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
        public Matrix NumericalDiff(int n, Vector range) {
            VectorRangeXY vr = new VectorRangeXY(range, this.GetRange(this.MaxN, epsilon), this.MaxN);
            Matrix em = this.EigenMatrix(n, range);

            Matrix result = new Matrix(numx, numy);

            for(int sx = 1; sx < vr.NumX - 1; sx++) {
                double x = vr.GetX(sx);
                for(int sy = 1; sy < vr.NumY - 1; sy++) {
                    double y = miny + koefy * sy;
                    double laplace = (em[sx + 1, sy] + em[sx - 1, sy] + em[sx, sy - 1] + em[sx, sy + 1]
                        - 4.0 * em[sx, sy]) / (koefx * koefy);

                    //laplace = 1 / (dx * dx) * (
                    //    -(M[i + 2, j] + M[i - 2, j] + M[i, j + 2] + M[i, j - 2]) / 12 +
                    //    4 * (M[i + 1, j] + M[i - 1, j] + M[i, j + 1] + M[i, j - 1]) / 3 - 5 * M[i, j]
                    //    ); //O(dx^4)

                    result[sx, sy] = -hbar * hbar / (2 * this.K) * laplace + em[sx, sy] * this.V(x, y) - em[sx, sy] * jacobi.EigenValue[n];
                    result[sx, sy] *= result[sx, sy];
                }
            }

            return result;
        }

        /// <summary>
        /// Funkce Psi_n (Form�nek 2.286)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Psi(int n, double x) {
            double xi = this.s * x;
            return this.n / System.Math.Sqrt(SpecialFunctions.Factorial(n) * System.Math.Pow(2, n)) * hermit.GetValue(n, xi) * System.Math.Exp(-xi * xi / 2);
        }

        /// <summary>
        /// Vrac� parametr range podle dosahu nejvy��� pou�it� vlastn� funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maxim�ln� rank vlastn� funkce</param>
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
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(this.A);
                b.Write(this.B);
                b.Write(this.C);
                b.Write(this.K);
                b.Write(this.A0);
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", this.A, this.B, this.C, this.K, this.A0);
            }

            export.Write(this.jacobi);
		}

		/// <summary>
		/// Na�te v�sledky ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                this.A = b.ReadDouble();
                this.B = b.ReadDouble();
                this.C = b.ReadDouble();
                this.K = b.ReadDouble();
                this.a0 = b.ReadDouble();
            }
            else {
                // Textov�
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
            this.hermit = new HermitPolynom((int)System.Math.Sqrt(this.jacobi.EigenValue.Length));
            this.RefreshConstants();
        }
        #endregion
    }
}