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
    public class LHOQuantumGCMA5D: LHOQuantumGCM {
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

        /// <summary>
        /// Konstruktor pro Import
        /// </summary>
        /// <param name="import">Import</param>
        public LHOQuantumGCMA5D(Core.Import import) : base(import) { }

        /// <summary>
        /// Vytvoøí instanci tøídy LHO5DIndex
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHO5DIndex(basisParams);
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        public double HamiltonianMatrixTrace(BasisIndex basisIndex) {
            LHO5DIndex index = basisIndex as LHO5DIndex;
            int maxE = index.MaxE;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;

            int length = this.eigenSystem.BasisIndex.Length;

            double result = 0.0;

            for(int i = 0; i < length; i++) {
                int l = index.L[i];
                int mu = index.Mu[i];

                int lambda = 3 * mu;
                double ro = lambda + 1.5;

                result += this.Hbar * omega * (2 * l + ro + 1);
                result += (this.A - this.A0) * (2 * l + ro + 1) / alpha;
                result += this.C * (l * (l - 1) + (l + ro + 1) * (5 * l + ro + 2)) / alpha2;
            }

            return result;
        }

        /// <summary>
        /// Hamiltonova matice v pásovém tvaru
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LHO5DIndex index = basisIndex as LHO5DIndex;
            int maxE = index.MaxE;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * System.Math.Sqrt(alpha);

            int length = index.Length;
            int bandWidth = index.BandWidth;

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("Pøíprava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int li = index.L[i];
                int mui = index.Mu[i];

                int lambdai = 3 * mui;
                double roi = lambdai + 1.5;

                for(int j = i; j < length; j++) {
                    int lj = index.L[j];
                    int muj = index.Mu[j];

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
                        matrix[i, j] = sum;
                        matrix[j, i] = sum;
                    }
                }
            }

            if(writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
        }

        protected override double PsiBG(double beta, double gamma, int n) {
            int l = (this.eigenSystem.BasisIndex as LHO5DIndex).L[n];
            int mu = (this.eigenSystem.BasisIndex as LHO5DIndex).Mu[n];

            double beta3 = beta * beta * beta;
            double sin3g = System.Math.Abs(System.Math.Sin(3.0 * gamma));
            double k = System.Math.Sqrt(beta3 * sin3g);

            return k * this.Psi5D(beta, l, mu) * this.Phi5D(gamma, mu);
        }

        /// <summary>
        /// Èasová støední hodnota druhého integrálu - Casimirùv operátor SO(5)
        /// </summary>
        protected override double PeresInvariantCoef(int n) {
            int lambda = 3 * (this.eigenSystem.BasisIndex as LHO5DIndex).Mu[n];
            return lambda * (lambda + 3) * this.Hbar * this.Hbar;
        }

        /// <summary>
        /// Druhý invariant pro operátor H0
        /// </summary>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        protected override Vector PeresInvariantHPrime() {
            LHO5DIndex index = this.eigenSystem.BasisIndex as LHO5DIndex;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * System.Math.Sqrt(alpha);

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                double sum = 0.0;

                for(int j = 0; j < length; j++) {
                    int l = index.L[j];
                    int mu = index.Mu[j];

                    int lambda = 3 * mu;
                    double ro = lambda + 1.5;

                    sum += ev[j] * ev[j] *
                            (this.Hbar * omega * (2.0 * l + ro + 1.0)
                                + (this.A - this.A0) * (2.0 * l + ro + 1.0) / alpha
                                + this.C * (l * (l - 1.0) + (l + ro + 1.0) * (5.0 * l + ro + 2.0)) / alpha2);

                    if(j < length - 1 && index.L[j + 1] == l + 1)
                        sum -= 2.0 * ev[j] * ev[j + 1] *
                                ((this.A - this.A0) * System.Math.Sqrt((l + 1.0) * (l + ro + 1.0)) / alpha
                                    + 2.0 * this.C * System.Math.Sqrt((l + 1.0) * (l + ro + 1.0)) * (2.0 * l + ro + 2.0) / alpha2);

                    if(j < length - 2 && index.L[j + 2] == l + 2)
                        sum += 2.0 * ev[j] * ev[j + 2] * this.C * System.Math.Sqrt((l + ro + 2.0) * (l + ro + 1.0) * (l + 2.0) * (l + 1.0)) / alpha2;
                }

                result[i] = sum;
            }

            return result;
        }

        /// <summary>
        /// Druhý invariant pro operátor H v oscilátorové bázi
        /// </summary>
        protected override Vector PeresInvariantHOscillator() {
            LHO5DIndex index = this.eigenSystem.BasisIndex as LHO5DIndex;
            
            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                int l = index.L[i];
                int mu = index.Mu[i];

                int lambda = 3 * mu;
                double ro = lambda + 1.5;

                result[i] += this.Hbar * omega * (2 * l + ro + 1);
                result[i] += (this.A - this.A0) * (2 * l + ro + 1) / alpha;
                result[i] += this.C * (l * (l - 1) + (l + ro + 1) * (5 * l + ro + 2)) / alpha2;
            }

            return result;
        }
    }
}