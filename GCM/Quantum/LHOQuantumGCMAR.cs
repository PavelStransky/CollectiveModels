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
        /// Vrátí èasovou støední hodnotu druhého integrálu - úhlového momentu -i * hbar * (d / d phi)
        /// (kvadratický Casimirùv operátor)
        /// </summary>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector GetSecondInvariant() {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenVectors[i];
                int length = ev.Length;

                for(int j = 0; j < length; j++) {
                    double d = ev[j] * this.index.M[j];
                    result[i] += d * d;
                }

                result[i] *= this.Hbar;
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
