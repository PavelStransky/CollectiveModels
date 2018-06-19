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
        private int length;

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

            this.length = System.Math.Min(2 * this.j + 1, this.m2 + 1);

            this.m = new int[this.length];
            this.nb = new int[this.length];

            for(int i = 0; i < length; i++) {
                this.m[i] = -this.j + i;
                this.nb[i] = this.m2 - i;
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
        public override int Length { get { return this.length; } }

        /// <summary>
        /// Šíøka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return System.Math.Min(3, this.length); } }

        /// <summary>
        /// Maximální hodnota poètu bosonù
        /// </summary>
        public int MaxNb { get { return this.m2; } }

        /// <summary>
        /// Minimální hodnota poètu bosonù
        /// </summary>
        public int MinNb { get { return System.Math.Max(0, this.m2 - 2 * this.j); } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly nb, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="nb">Poèet bosonù</param>
        /// <param name="m">Orbitální kvantové èíslo</param>
        public int this[int nb, int m] {
            get {
                if(nb + m + this.j != this.m2)
                    return -1;
                if(nb < 0 || m < -this.j || m > this.j)
                    return -1;
                return m + this.j;                
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