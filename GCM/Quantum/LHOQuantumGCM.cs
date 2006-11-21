using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
    /// </summary>
    public class LHOQuantumGCM : GCM, IExportable {
        private double hbar = 1.0E-1;	// [Js]

        // Koeficienty
        private double s, n;

        // Parametr pro LHO
        private double a0;

        // Hermit�v polynom
        private HermitPolynom hermit;
        // Cache psi hodnot (Bazove vlnove funkce)
        private BasisCache psiCache;
        // Cache hodnot potencialu
        private double[,] vCache;

        Jacobi jacobi;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } set { this.hbar = value; } }

        /// <summary>
        /// �hlov� frekvence LHO [J*m^-2]
        /// </summary>
        public double Omega { get { return System.Math.Sqrt(2.0 * this.a0 / this.K); } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// Meze pro integraci
        /// </summary>
        public double Range { get { return 15.0 / this.s; } }

        /// <summary>
        /// Pr�zdn� konstruktor
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
        /// P�epo��t� konstanty s, n
        /// </summary>
        private void RefreshConstants() {
            // Konstanty
            this.s = System.Math.Sqrt(this.K * this.Omega / this.hbar);      // xi = s*x (Formanek (2.283))
            this.n = System.Math.Sqrt(System.Math.Sqrt(this.K * this.Omega / (System.Math.PI * this.hbar)));
                                                        // sqrt(sqrt(M*omega/(pi*hbar))) (Formanek (2.286))
        }

        /// <summary>
        /// Vytvo�� cache
        /// </summary>
        /// <param name="maxn">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        private void CreateCaches(int maxn, int numSteps) {
            double range = this.Range;
            double step = 2.0 * range / numSteps;

            // Cachov�n� b�zov�ch funkc�
            this.hermit = new HermitPolynom(maxn);
            this.psiCache = new BasisCache(-range, step, numSteps, maxn, this.Psi);

            // Cachov�n� potenci�lu
            this.vCache = new double[numSteps, numSteps];
            for(int sy = 0; sy < numSteps; sy++)
                for(int sx = 0; sx < numSteps; sx++)
                    this.vCache[sx, sy] = this.V(-range + step * sx, -range + step * sy);
        }

        /// <summary>
        /// Provede v�po�et
        /// </summary>
        /// <param name="maxn">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Wirter</param>
        public void Compute(int maxn, int numSteps, IOutputWriter writer) {
            double omega = this.Omega;
            double step = 2.0 * this.Range / numSteps;

            if(writer != null)
                writer.WriteLine("P�ipravuji cache...");

            this.CreateCaches(maxn, numSteps);

            int max2 = maxn * maxn;
            Matrix m = new Matrix(max2, max2);

            if(writer != null)
                writer.WriteLine(string.Format("P��prava H ({0} x {1})", max2, max2));

            for(int i = 0; i < max2; i++) {
                for(int j = 0; j < max2; j++) {
                    int ix = i / maxn;
                    int iy = i % maxn;
                    int jx = j / maxn;
                    int jy = j % maxn;

                    double sum = 0;
                    for(int sy = 0; sy < numSteps; sy++)
                        for(int sx = 0; sx < numSteps; sx++)
                            sum += this.psiCache[ix, sx] * this.psiCache[iy, sy] * this.vCache[sx, sy] * 
                                this.psiCache[jx, sx] * this.psiCache[jy, sy];

                    sum *= step * step;

                    if(ix == jx && iy == jy)
                        sum += this.hbar * omega * (0.5 + ix + iy);

                    m[i, j] = sum;
                }

                // V�pis na konzoli
                if(writer != null) {
                    if(i % maxn == 0 && i != 0)
                        writer.WriteLine();

                    writer.Write(".");
                }
            }

            writer.WriteLine();

            if(writer != null)
                writer.WriteLine("Diagonalizace...");

            this.jacobi = new Jacobi(m);
            this.jacobi.SortAsc();
        }

        /// <summary>
        /// Vlastn� hodnoty
        /// </summary>
        public double[] EigenValue { get { return this.jacobi.EigenValue; } }

        /// <summary>
        /// Vlastn� vektory
        /// </summary>
        public Vector[] EigenVector { get { return this.jacobi.EigenVector; } }

        /*
        /// <summary>
        /// Vr�t� matici 
        /// </summary>
        /// <param name="eigenIndex"></param>
        /// <returns></returns>
        public Matrix EigenVectorDensity(int eigenIndex) {
            Vector ev = jacobi.EigenVector[eigenIndex];
            Matrix result = new Matrix(numSteps);

            double Vmin = 0;
            double range = this.Range;
            double step = 2.0 * range / numSteps;

            int length = ev.Length;
            for(int i = 0; i < length; i++) {
                int ix = i / maxn;
                int iy = i % maxn;
                double y = -range;

                for(int c = 0; c < steps; c++) {

                    double x = -range;

                    for(int d = 0; d < steps; d++) {

                        // d ~ x, c ~ y
                        M[d, c] += PsiCached[ix].Value(x) * PsiCached[iy].Value(y) * v[i];
                        x += dx;
                        if(V(x, y) < Vmin) Vmin = V(x, y);
                    }

                    y += dx;
                }

            } // for (i = ...

            //        Console.WriteLine("Vmin = {0}", Vmin);

            for(int c = 0; c < steps; c++)
                for(int d = 0; d < steps; d++)
                    M[c, d] = Math.Max(M[c, d] * M[c, d], 0 * Math.Min(V(-range + c * dx, -range + d * dx) + 3, 5));
            return M;

        }
        */

        /// <summary>
        /// Funkce Psi_n (Form�nek 2.286)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Psi(int n, double x) {
            double xi = this.s * x;
            return this.n / System.Math.Sqrt(SpecialFunctions.Factorial(n) * System.Math.Pow(2, n)) * this.hermit.GetValue(n, xi) * System.Math.Exp(-xi * xi / 2);
        }

        private const int maxn = 25;        // max^2 je velikost vypocetni baze
        private const int numSteps = 200;	// pocet bodu pri diskretizaci psi

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

                b.Write(this.psiCache.MaxIndex);
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", this.A, this.B, this.C, this.K, this.A0);
                t.WriteLine(this.psiCache.MaxIndex);
            }

            export.Write(this.jacobi);
		}

		/// <summary>
		/// Na�te v�sledky ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            int numSteps = 0;

            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                this.A = b.ReadDouble();
                this.B = b.ReadDouble();
                this.C = b.ReadDouble();
                this.K = b.ReadDouble();
                this.a0 = b.ReadDouble();

                numSteps = b.ReadInt32();
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

                numSteps = int.Parse(t.ReadLine());
            }

            this.jacobi = import.Read() as Jacobi;
            this.RefreshConstants();
            this.CreateCaches(maxn, numSteps);
        }
        #endregion
    }
}