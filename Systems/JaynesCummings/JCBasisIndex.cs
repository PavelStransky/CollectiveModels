using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy pro Jaynes-Cummingsùv model
    /// </summary>
    public class JaynesCummingsBasisIndex: BasisIndex {
        private int j, m2;

        private int[] nb, m;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public JaynesCummingsBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public JaynesCummingsBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.m2 = (int)basisParams[0];
            this.j = (int)basisParams[1];

            int minnb = System.Math.Max(0, (this.m2 - 4 * this.j) / 2);
            int maxnb = this.m2 / 2;

            int length = 0;
            for(int i = minnb; i <= maxnb; i++) {
                int m1 = this.m2 / 2 - i - this.j;
                if(m1 >= -this.j && m1 <= this.j)
                    length++;
            }

            this.m = new int[length];
            this.nb = new int[length];
            length = 0;
            for(int i = minnb; i <= maxnb; i++) {
                int m1 = this.m2 / 2 - i - this.j;
                if(m1 >= -this.j && m1 <= this.j) {
                    this.m[length] = m1;
                    this.nb[length] = i;
                    length++;
                }
            }
        }

        /// <summary>
        /// Poèet bosonù
        /// </summary>
        public int[] Nb { get { return this.nb; } }

        /// <summary>
        /// Orbitální kvantové èíslo
        /// </summary>
        public int[] M { get { return this.m; } }

        /// <summary>
        /// Druhý integrál pohybu M2
        /// </summary>
        public int M2 { get { return this.m2; } }

        /// <summary>
        /// Dvakrát poèet atomù
        /// </summary>
        public int J { get { return this.j; } }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return 1; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length { get { return this.nb.Length; } }

        /// <summary>
        /// Šíøka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return 3; } }

        /// <summary>
        /// Maximální hodnota poètu bosonù
        /// </summary>
        public int MaxL { get { return this.nb[this.Length - 1]; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly nb, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="nb">Poèet bosonù</param>
        /// <param name="m">Orbitální kvantové èíslo</param>
        public int this[int nb, int m] {
            get {
                for(int i = 0; i < this.Length; i++)
                    if(this.nb[i] == nb && this.m[i] == m)
                        return i;
                return -1;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m1, m2.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            switch(qn) {
                case 0:
                    return this.Length;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.nb[i];
                case 1:
                    return this.m[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}