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
    public class LHOQuantumGCMARO: LHOQuantumGCM {
        // Indexy b�ze
        protected LHOPolarIndex index;
        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LHOQuantumGCMARO() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMARO(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vytvo�� instanci t��dy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        protected virtual void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE, false, 1);
        }

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        public override int HamiltonianMatrixSize(int maxE) {
            this.CreateIndex(maxE);
            return this.index.Length;
        }

        public override double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;

            int length = this.index.Length;

            double result = 0.0;

            for(int i = 0; i < length; i++) {
                int n = this.index.N[i];
                int m = this.index.M[i];

                int l = System.Math.Abs(m);

                result += this.Hbar * omega * (2 * n + l + 1);
                result += (this.A - this.A0) * (2 * n + l + 1) / alpha;
                result += this.C * (n * (n - 1) + (n + l + 1) * (5 * n + l + 2)) / alpha2;
            }

            return result;
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="writer">Writer</param>
        protected override SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
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
                    double n = System.Math.Min(ni, nj);         // Mus�me po��tat jako double (jinak dojde k p�ete�en�)

                    int diffl = System.Math.Abs(mi - mj);
                    double l = System.Math.Min(li, lj);

                    double sum = 0.0;

                    // V�b�rov� pravidlo
                    if(diffl == 0) {
                        if(diffn == 0) {
                            sum += this.Hbar * omega * (2.0 * n + l + 1.0);
                            sum += (this.A - this.A0) * (2.0 * n + l + 1.0) / alpha;
                            sum += this.C * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0)) / alpha2;
                        }

                        else if(diffn == 1) {
                            sum -= (this.A - this.A0) * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) / alpha;
                            sum -= 2.0 * this.C * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0) / alpha2;
                        }

                        else if(diffn == 2)
                            sum += this.C * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0)) / alpha2;
                    }

                    else if(diffl == 3 && ((mi > mj && ni <= nj) || (mi < mj && ni >= nj))) {
                        if(diffn == 0)
                            sum += 0.5 * System.Math.Sqrt((n + l + 3.0) * (n + l + 2.0) * (n + l + 1.0)) / alpha32;
                        else if(diffn == 1)
                            sum -= 0.5 * 3.0 * System.Math.Sqrt((n + 1.0) * (n + l + 3.0) * (n + l + 2.0)) / alpha32;
                        else if(diffn == 2)
                            sum += 0.5 * 3.0 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 3.0)) / alpha32;
                        else if(diffn == 3)
                            sum -= 0.5 * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0)) / alpha32;
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

        public LHOQuantumGCMARO(Core.Import import) : base(import) { }
        #endregion
    }
}