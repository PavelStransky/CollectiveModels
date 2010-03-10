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

        /// <summary>
        /// Konstruktor pro Import
        /// </summary>
        /// <param name="import">Import</param>
        public LHOQuantumGCMARE(Core.Import import) : base(import) { }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHOPolarIndexE(basisParams);
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        public override double HamiltonianMatrixTrace(BasisIndex basisIndex) {
            LHOPolarIndexE index = basisIndex as LHOPolarIndexE;
            int maxE = index.MaxE;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;

            int length = index.Length;

            double result = 0.0;

            for(int i = 0; i < length; i++) {
                int n = index.N[i];
                int m = index.M[i];

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
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public override SymmetricBandMatrix HamiltonianSBMatrix(BasisIndex basisIndex, IOutputWriter writer) {
            LHOPolarIndexE index = basisIndex as LHOPolarIndexE;
            int maxE = index.MaxE;
            
            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int length = index.Length;
            int bandWidth = index.BandWidth;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("Pøíprava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int ni = index.N[i];
                int mi = index.M[i];

                int li = System.Math.Abs(mi);

                for(int j = i; j < length; j++) {
                    int nj = index.N[j];
                    int mj = index.M[j];

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
            LHOPolarIndex index = this.eigenSystem.BasisIndex as LHOPolarIndex;
            int n = index.N[l];
            int m = index.M[l];

            return this.Psi2D(beta, n, m) * this.Phi2DE(gamma, m);
        }
    }
}