using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje index 
    /// pro b�zi 1D line�rn�ho harmonick�ho oscil�toru
    /// </summary>
    public class PTBasisIndex: BasisIndex {
        private int maxn;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PTBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public PTBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.maxn = (int)basisParams[0];                     // Maxim�ln� energie v n�sobc�ch hbar * Omega
        }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length { get { return this.maxn; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla N
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrac� index prvku 
        /// </summary>
        /// <param name="basisIndex">Vektor s indexem</param>
        public override int this[Vector basisIndex] {
            get {
                return (int)basisIndex[0];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn == 0)
                return this.maxn;

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if(qn == 0)
                return i;

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}
