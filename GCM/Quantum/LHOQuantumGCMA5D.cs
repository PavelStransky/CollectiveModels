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
    public class LHOQuantumGCMA5D: LHOQuantumGCM {
        // Indexy b�ze
        protected LHO5DIndex index;
        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LHOQuantumGCMA5D() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMA5D(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vytvo�� instanci t��dy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        protected virtual void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHO5DIndex(maxE);
        }

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        public override int HamiltonianMatrixSize(int maxE) {
            this.CreateIndex(maxE);
            return this.index.Length;
        }

        protected override SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * System.Math.Sqrt(alpha);

            int length = this.index.Length;
            int bandWidth = maxE;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("P��prava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int li = this.index.L[i];
                int mui = this.index.Mu[i];

                int lambdai = 3 * mui;
                double roi = lambdai + 1.5;

                for(int j = i; j < length; j++) {
                    int lj = this.index.L[j];
                    int muj = this.index.Mu[j];

                    int lambdaj = 3 * muj;
                    double roj = lambdaj + 1.5;

                    int diffl = System.Math.Abs(li - lj);
                    int l = System.Math.Min(li, lj);

                    int difflambda = System.Math.Abs(lambdai - lambdaj);
                    int mu = System.Math.Min(mui, muj);
                    double ro = System.Math.Min(roi, roj);

                    double sum = 0.0;

                    // V�b�rov� pravidlo
                    if(difflambda == 0) {
                        if(diffl == 0) {
                            sum += this.Hbar * omega * (2 * l + ro + 1);
                            sum += (this.A - this.A0) * (2 * l + ro + 1) / alpha;
                            sum += this.C * (l * (l - 1) + (l + ro + 1) * (5 * l + ro + 2)) / alpha2;
                        }

                        else if(diffl == 1) {
                            sum -= (this.A - this.A0) * System.Math.Sqrt((l + 1) * (l + ro + 1)) / alpha;
                            sum -= 2.0 * this.C * System.Math.Sqrt((l + 1) * (l + ro + 1)) * (2 * l + ro + 2) / alpha2;
                        }

                        else if(diffl == 2)
                            sum += this.C * System.Math.Sqrt((l + ro + 2) * (l + ro + 1) * (l + 2) * (l + 1)) / alpha2;
                    }

                    else if(difflambda == 3 && ((mui > muj && li <= lj) || (mui < muj && li >= lj))) {
                        double k = (mu + 1) / System.Math.Sqrt((2 * mu + 1) * (2 * mu + 3));

                        if(diffl == 0)
                            sum += k * System.Math.Sqrt((l + ro + 3) * (l + ro + 2) * (l + ro + 1)) / alpha32;
                        else if(diffl == 1)
                            sum -= k * 3.0 * System.Math.Sqrt((l + 1) * (l + ro + 3) * (l + ro + 2)) / alpha32;
                        else if(diffl == 2)
                            sum += k * 3.0 * System.Math.Sqrt((l + 2) * (l + 1) * (l + ro + 3)) / alpha32;
                        else if(diffl == 3)
                            sum -= k * System.Math.Sqrt((l + 3) * (l + 2) * (l + 1)) / alpha32;
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

        public LHOQuantumGCMA5D(Core.Import import) : base(import) { }
        #endregion
    }
}