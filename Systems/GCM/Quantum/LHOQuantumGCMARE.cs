using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// - užití algebraických vztahù namísto integrace
    /// </summary>
    public class LHOQuantumGCMARE: LHOQuantumGCMAR {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LHOQuantumGCMARE() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMARE(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        protected override int GetBasisQuantumNumber2(int i) {
            if(i < 0)
                return this.index.MaxM / 3 + 2;
            else
                return this.index.M[i] / 3;
        }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        protected override void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE, false, 2);
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
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="maxE">Nejvyšší energie v násobcích hbar * Omega</param>
        /// <param name="writer">Writer</param>
        protected override SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int length = this.index.Length;
            int bandWidth = maxE - 2;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("Pøíprava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                int li = System.Math.Abs(mi);

                for(int j = i; j < length; j++) {
                    int nj = this.index.N[j];
                    int mj = this.index.M[j];

                    int lj = System.Math.Abs(mj);

                    int diffn = System.Math.Abs(ni - nj);
                    double n = System.Math.Min(ni, nj);         // Musíme poèítat jako double (jinak dojde k pøeteèení)

                    int diffl = System.Math.Abs(mi - mj);
                    double l = System.Math.Min(li, lj);

                    double sum = 0.0;

                    // Výbìrové pravidlo
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
                        double k = (mi == 0 || mj == 0) ? 1.0 / System.Math.Sqrt(2.0) : 0.5;
                        k *= this.B;

                        if(diffn == 0)
                            sum += k * System.Math.Sqrt((n + l + 3.0) * (n + l + 2.0) * (n + l + 1.0)) / alpha32;
                        else if(diffn == 1)
                            sum -= k * 3.0 * System.Math.Sqrt((n + 1.0) * (n + l + 3.0) * (n + l + 2.0)) / alpha32;
                        else if(diffn == 2)
                            sum += k * 3.0 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 3.0)) / alpha32;
                        else if(diffn == 3)
                            sum -= k * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0)) / alpha32;
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
        /// Vlnová funkce ve 2D
        /// </summary>
        protected override double PsiBG(double beta, double gamma, int l) {
            int n = this.index.N[l];
            int m = this.index.M[l];

            return this.Psi2D(beta, n, m) * this.Phi2DE(gamma, m);
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

        public LHOQuantumGCMARE(Core.Import import) : base(import) { }
        #endregion
    }
}