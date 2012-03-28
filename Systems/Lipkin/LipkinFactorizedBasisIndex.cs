using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy pro Lipkinùv model diagonalizovaný v bázi jednotlivých spiníèkù
    /// </summary>
    public class LipkinFactorizedBasisIndex: BasisIndex {
        private int n;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFactorizedBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinFactorizedBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.n = (int)basisParams[0];
        }

        /// <summary>
        /// Poèet bosonù
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return this.n; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length {
            get {
                return 1 << this.n;
            }
        }

        /// <summary>
        /// Šíøka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return this.Length; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m1, m2.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                int result = 0;
                for(int i = 0; i < basisIndex.Length; i++) {
                    if(basisIndex[i] == 1)
                        result |= 1 << i;
                    else if(basisIndex[i] != 0)
                        return -1;
                }

                return result;
            }
        }

        public string Ket(int i) {
            StringBuilder result = new StringBuilder();
            result.Append('|');
            for(int j = 0; j < this.n; j++)
                if((i & (1 << j)) == 0)
                    result.Append('b');
                else
                    result.Append('t');
            result.Append('>');
            return result.ToString();
        }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn < this.n)
                return 1;

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if(qn < this.n)
                return (i & (1 << qn)) >> qn;

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}