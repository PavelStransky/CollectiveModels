using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje index 
    /// pro bázi 1D lineárního harmonického oscilátoru
    /// </summary>
    public class PTBasisIndex: BasisIndex {
        private int maxn;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PTBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public PTBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.maxn = (int)basisParams[0];                     // Maximální energie v násobcích hbar * Omega
        }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length { get { return this.maxn; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla N
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrací index prvku 
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
