using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// - užití algebraických vztahù namísto integrace
    /// </summary>
    public class LHOQuantumGCMA5D: LHOQuantumGCM {
        // Indexy báze
        protected LHO5DIndex index;
        /// <summary>
        /// Prázdný konstruktor
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

        protected override int GetBasisLength() {
            return this.index.Length;
        }

        protected override int GetBasisQuantumNumber1(int i) {
            if(i < 0)
                return this.index.MaxL + 1;
            else
                return this.index.L[i];
        }

        protected override int GetBasisQuantumNumber2(int i) {
            if(i < 0)
                return this.index.MaxMu + 1;
            else
                return this.index.Mu[i];
        }

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

        public override double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;

            int length = this.index.Length;

            double result = 0.0;

            for(int i = 0; i < length; i++) {
                int l = this.index.L[i];
                int mu = this.index.Mu[i];

                int lambda = 3 * mu;
                double ro = lambda + 1.5;

                result += this.Hbar * omega * (2 * l + ro + 1);
                result += (this.A - this.A0) * (2 * l + ro + 1) / alpha;
                result += this.C * (l * (l - 1) + (l + ro + 1) * (5 * l + ro + 2)) / alpha2;
            }

            return result;
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
                writer.Write(string.Format("Pøíprava H ({0} x {1})...", length, length));

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
                    double l = System.Math.Min(li, lj);         // Musíme poèítat jako double (jinak dojde k pøeteèení)

                    int difflambda = System.Math.Abs(lambdai - lambdaj);
                    double mu = System.Math.Min(mui, muj);
                    double ro = System.Math.Min(roi, roj);

                    double sum = 0.0;

                    // Výbìrové pravidlo
                    if(difflambda == 0) {
                        if(diffl == 0) {
                            sum += this.Hbar * omega * (2.0 * l + ro + 1.0);
                            sum += (this.A - this.A0) * (2.0 * l + ro + 1.0) / alpha;
                            sum += this.C * (l * (l - 1.0) + (l + ro + 1.0) * (5.0 * l + ro + 2.0)) / alpha2;
                        }

                        else if(diffl == 1) {
                            sum -= (this.A - this.A0) * System.Math.Sqrt((l + 1.0) * (l + ro + 1.0)) / alpha;
                            sum -= 2.0 * this.C * System.Math.Sqrt((l + 1.0) * (l + ro + 1.0)) * (2.0 * l + ro + 2.0) / alpha2;
                        }

                        else if(diffl == 2)
                            sum += this.C * System.Math.Sqrt((l + ro + 2.0) * (l + ro + 1.0) * (l + 2.0) * (l + 1.0)) / alpha2;
                    }

                    else if(difflambda == 3 && ((mui > muj && li <= lj) || (mui < muj && li >= lj))) {
                        double k = this.B * (mu + 1.0) / System.Math.Sqrt((2.0 * mu + 1.0) * (2.0 * mu + 3.0));

                        if(diffl == 0)
                            sum += k * System.Math.Sqrt((l + ro + 3.0) * (l + ro + 2.0) * (l + ro + 1.0)) / alpha32;
                        else if(diffl == 1)
                            sum -= k * 3.0 * System.Math.Sqrt((l + 1.0) * (l + ro + 3.0) * (l + ro + 2.0)) / alpha32;
                        else if(diffl == 2)
                            sum += k * 3.0 * System.Math.Sqrt((l + 2.0) * (l + 1.0) * (l + ro + 3.0)) / alpha32;
                        else if(diffl == 3)
                            sum -= k * System.Math.Sqrt((l + 3.0) * (l + 2.0) * (l + 1.0)) / alpha32;
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

        protected override double PsiBG(double beta, double gamma, int n) {
            int l = this.index.L[n];
            int mu = this.index.Mu[n];

            double beta3 = beta * beta * beta;
            double sin3g = System.Math.Abs(System.Math.Sin(3.0 * gamma));
            double k = System.Math.Sqrt(beta3 * sin3g);

            return k * this.Psi5D(beta, l, mu) * this.Phi5D(gamma, mu);
        }

        /// <summary>
        /// Èasová støední hodnota druhého integrálu - Casimirùv operátor SO(5)
        /// </summary>
        protected override double SecondInvariantCoef(int n) {
            int lambda = 3 * this.index.Mu[n];
            return lambda * (lambda + 3) * this.Hbar * this.Hbar;
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