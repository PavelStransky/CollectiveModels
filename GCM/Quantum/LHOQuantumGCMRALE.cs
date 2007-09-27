using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
    /// - u�it� algebraick�ch vztah� nam�sto integrace
    /// </summary>
    public class LHOQuantumGCMRALE: LHOQuantumGCM {
        // Indexy b�ze
        protected LHOPolarIndex index;
        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LHOQuantumGCMRALE() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCMRALE(double a, double b, double c, double k, double a0)
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
        public LHOQuantumGCMRALE(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vytvo�� instanci t��dy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        protected virtual void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE, false, 2);
        }

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        public override int HamiltonianMatrixSize(int maxE) {
            this.CreateIndex(maxE);
            return this.index.Length;
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Nejvy��� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public override double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            return this.HamiltonianMatrix(maxE, numSteps, null).Trace();
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="writer">Writer</param>
        public override Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(maxE, numSteps, writer);
        }

        protected virtual SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * System.Math.Sqrt(alpha);

            int length = this.index.Length;
            int bandWidth = maxE - 2;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("P��prava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                int li = System.Math.Abs(mi);

                for(int j = i; j < length; j++) {
                    int nj = this.index.N[j];
                    int mj = this.index.M[j];

                    int lj = System.Math.Abs(mj);

                    int diffn = System.Math.Abs(ni - nj);
                    int n = System.Math.Min(ni, nj);

                    int diffl = System.Math.Abs(mi - mj);
                    int l = System.Math.Min(li, lj);

                    double sum = 0.0;

                    // V�b�rov� pravidlo
                    if(diffl == 0) {
                        if(diffn == 0) {
                            sum += this.Hbar * omega * (2 * n + l + 1);
                            sum += (this.A - this.A0) * (2 * n + l + 1) / alpha;
                            sum += this.C * (n * (n - 1) + (n + l + 1) * (5 * n + l + 2)) / alpha2;
                        }

                        else if(diffn == 1) {
                            sum -= (this.A - this.A0) * System.Math.Sqrt((n + 1) * (n + l + 1)) / alpha;
                            sum -= 2.0 * this.C * System.Math.Sqrt((n + 1) * (n + l + 1)) * (2 * n + l + 2) / alpha2;
                        }

                        else if(diffn == 2)
                            sum += this.C * System.Math.Sqrt((n + l + 2) * (n + l + 1) * (n + 2) * (n + 1)) / alpha2;
                    }

                    else if(diffl == 3 && ((mi > mj && ni <= nj) || (mi < mj && ni >= nj))) {
                        double k = (mi == 0 || mj == 0) ? 1.0 / System.Math.Sqrt(2.0) : 0.5;

                        if(diffn == 0)
                            sum += k * System.Math.Sqrt((n + l + 3) * (n + l + 2) * (n + l + 1)) / alpha32;
                        else if(diffn == 1)
                            sum -= k * 3.0 * System.Math.Sqrt((n + 1) * (n + l + 3) * (n + l + 2)) / alpha32;
                        else if(diffn == 2)
                            sum += k * 3.0 * System.Math.Sqrt((n + 2) * (n + 1) * (n + l + 3)) / alpha32;
                        else if(diffn == 3)
                            sum -= k * System.Math.Sqrt((n + 3) * (n + 2) * (n + 1)) / alpha32;
                    }

                    if(sum != 0.0) {
                        m[i, j] = sum;
                        m[j, i] = sum;
                    }
                }
            }

            if(writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return m;
        }

        /// <summary>
        /// Provede v�po�et
        /// </summary>
        /// <param name="maxE">Nejvy��� energie b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public override void Compute(int maxE, int numSteps, bool ev, int numev, IOutputWriter writer) {
            if(this.isComputing)
                throw new GCMException(errorMessageComputing);
            this.isComputing = true;

            GC.Collect();

            if(numev <= 0 || numev > this.HamiltonianMatrixSize(maxE))
                numev = this.HamiltonianMatrixSize(maxE);

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine(string.Format("{0} ({1}): V�po�et {2} vlastn�ch hodnot{3}.",
                    this.GetType().Name,
                    startTime,
                    numev,
                    ev ? " a vektor�" : string.Empty));
                writer.Indent(1);
                writer.WriteLine(string.Format("Maxim�ln� (n, m) = ({0}, {1})", this.index.MaxN, this.index.MaxM));
                writer.WriteLine(string.Format("Velikost b�ze: {0}", this.index.Length));
            }

            SymmetricBandMatrix m = this.HamiltonianSBMatrix(maxE, numSteps, writer);

            if(writer != null) {
                writer.WriteLine(string.Format("Stopa matice: {0}", m.Trace()));
                writer.WriteLine("�i�t�n� pam�ti...");
            }

            // Mus�me uvolnit co nejv�ce pam�ti
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
                writer.WriteLine(string.Format("Sou�et vlastn�ch ��sel: {0}", this.eigenValues.Sum()));
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            this.isComputed = true;
            this.isComputing = false;
        }

        /// <summary>
        /// Vr�t� matici hustot pro vlastn� funkce
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="interval">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
        public override Matrix DensityMatrix(int n, params Vector[] interval) {
            return null;
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

        public LHOQuantumGCMRALE(Core.Import import) : base(import) { }
        #endregion
    }
}