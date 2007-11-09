using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    public abstract class LHOQuantumGCMAR: LHOQuantumGCM {
        // Indexy b�ze
        protected LHOPolarIndex index;
        /// <summary>
        /// Pr�zdn� konstruktor
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
        /// Vytvo�� instanci t��dy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        protected abstract void CreateIndex(int maxE);

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
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
        /// Vr�t� �asovou st�edn� hodnotu druh�ho integr�lu - �hlov�ho momentu -i * hbar * (d / d phi)
        /// (kvadratick� Casimir�v oper�tor)
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
    }
}