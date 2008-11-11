using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    public abstract class LHOQuantumGCMAR: LHOQuantumGCM {
        // Indexy báze
        protected LHOPolarIndex index;
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LHOQuantumGCMAR() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMAR(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        protected LHOQuantumGCMAR(Core.Import import) : base(import) { }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        protected abstract void CreateIndex(int maxE);

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        public override int HamiltonianMatrixSize(int maxE) {
            this.CreateIndex(maxE);
            return this.index.Length;
        }

        protected override int GetBasisLength() {
            return this.index.Length;
        }

        protected override int GetBasisQuantumNumber1(int i) {
            if(i < 0)
                return this.index.MaxN + 1;
            else
                return this.index.N[i];
        }

        /// <summary>
        /// Èasová støední hodnota druhého integrálu - Casimirùv operátor SO(2) hbar^2 * (d / d phi)^2
        /// </summary>
        protected override double PeresInvariantCoef(int n) {
            double d = this.index.M[n] * this.Hbar;
            return d * d;
        }

        /// <summary>
        /// Druhý invariant pro operátor H0
        /// </summary>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public override Vector GetPeresInvariantH0() {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenVectors[i];
                int length = ev.Length;

                double sum = 0.0;

                for(int j = 0; j < length; j++) {
                    int n = this.index.N[j];
                    int m = this.index.M[j];

                    int l = System.Math.Abs(m);

                    sum += ev[j] * ev[j] *
                            (this.Hbar * omega * (2.0 * n + l + 1.0)
                                + (this.A - this.A0) * (2.0 * n + l + 1.0) / alpha
                                + this.C * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0)) / alpha2);

                    if(j < length - 1 && this.index.N[j + 1] == n + 1)
                        sum -= 2.0 * ev[j] * ev[j + 1] *
                                ((this.A - this.A0) * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) / alpha
                                    + 2.0 * this.C * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0) / alpha2);

                    if(j < length - 2 && this.index.N[j + 2] == n + 2) 
                        sum += 2.0 * ev[j] * ev[j + 2] * this.C * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0)) / alpha2;
                }

                result[i] = sum;
            }

            return result;
        }

        /// <summary>
        /// Druhý invariant pro operátor H0 
        /// (zkouška poèítat pøímo <beta^3 cos(3 gamma)>, zatím nefunguje)
        /// </summary>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector GetPeresInvariantH01() {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int f = 0; f < count; f++) {
                Vector ev = this.eigenVectors[f];
                int length = ev.Length;

                double sum = 0.0;

                for(int i = 0; i < length; i++) {
                    int n = this.index.N[i];
                    int m = this.index.M[i];

                    int l = System.Math.Abs(m);

                    int f0 = this.index[n, m + 3];
                    if(f0 < 0) 
                        continue;

                    double k = (m == 0) ? 1.0 / System.Math.Sqrt(2.0) : 0.5;

                    sum += ev[f] * ev[f0] * System.Math.Sqrt((n + l + 3.0) * (n + l + 2.0) * (n + l + 1.0)) / alpha32;

                    if(f0 < length - 1 && this.index.N[f0 + 1] == n + 1)
                        sum -= ev[f] * ev[f0 + 1] * k * 3.0 * System.Math.Sqrt((n + 1.0) * (n + l + 3.0) * (n + l + 2.0)) / alpha32;

                    if(f0 < length - 2 && this.index.N[f0 + 2] == n + 2)
                        sum += ev[f] * ev[f0 + 2] * k * 3.0 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 3.0)) / alpha32;

                    if(f0 < length - 3 && this.index.N[f0 + 3] == n + 3)
                        sum -= ev[f] * ev[f0 + 3] * k * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0)) / alpha32;
                }

                result[f] = sum;
            }

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="m">Spin</param>
        /// <param name="r">Radiální souøadnice</param>
        public double Psi(double r, int n, int m) {
            double xi2 = this.s * r; xi2 *= xi2;
            m = System.Math.Abs(m);

            double normLog = 0.5 * (System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n) - SpecialFunctions.FactorialILog(n + m)) + (m + 1) * System.Math.Log(this.s);
            double l = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out l, out e, xi2, n, m);

            if(l == 0.0 || r == 0.0)
                return 0.0;

            double lLog = System.Math.Log(System.Math.Abs(l));
            double result = normLog + m * System.Math.Log(r) - xi2 / 2.0 + lLog + e;
            result = l < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="i">Index (kvantová èísla zjistíme podle uchované cache indexù)</param>
        /// <param name="x">Souøadnice</param>
        protected double Psi(double x, int i) {
            return this.Psi(x, this.index.N[i], this.index.M[i]);
        }
    }
}
