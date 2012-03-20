using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy pro Lipkinùv model diagonalizovaný v bázi SU(2)
    /// </summary>
    public class LipkinFullBasisIndex: BasisIndex {
        private int n;

        private int[] l, m;
        private bool fixL;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFullBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinFullBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.n = (int)basisParams[0];

            // Druhý index: hodnota kvantového èísla l
            if(basisParams.Length > 1) {
                this.fixL = true;

                this.l = new int[this.n + 1];
                this.m = new int[this.n + 1];

                this.l[0] = (int)basisParams[1];

                int num = 0;
                for(int j = -this.n; j <= this.n; j += 2) {
                    this.l[num] = this.l[0];
                    this.m[num] = j;
                    num++;
                }
            }
            else {
                this.fixL = false;

                // Poèet prvkù báze (na toto nejspíš existuje vzoreèák)
                int num = 0;
                for(int i = this.n; i >= 0; i -= 2)
                    for(int j = -i; j <= i; j += 2)
                        num++;

                this.l = new int[num];
                this.m = new int[num];

                num = 0;
                for(int i = this.n; i >= 0; i -= 2)
                    for(int j = -i; j <= i; j += 2) {
                        this.l[num] = i;
                        this.m[num] = j;
                        num++;
                    }
            }
        }

        /// <summary>
        /// Hlavní kvantové èíslo
        /// </summary>
        public int[] L { get { return this.l; } }

        /// <summary>
        /// Orbitální kvantové èíslo
        /// </summary>
        public int[] M { get { return this.m; } }

        /// <summary>
        /// Poèet bosonù
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return this.fixL ? 1 : 2; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length {
            get {
                return this.l.Length;
            }
        }

        /// <summary>
        /// Šíøka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return 3; } }

        /// <summary>
        /// Maximální hodnota hlavního kvantového èísla l
        /// </summary>
        public int MaxL { get { return this.fixL ? this.l[0] : this.N; } }

        /// <summary>
        /// Maximální hodnota orbitálního kvantového èísla m
        /// </summary>
        public int MaxM { get { return this.N; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly l, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="l">Hlavní kvantové èíslo</param>
        /// <param name="m">Orbitální kvantové èíslo</param>
        public int this[int m, int l] {
            get {
                for(int i = 0; i < this.Length; i++)
                    if(this.m[i] == m && this.l[i] == l)
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
                    return this.n + 1;
                case 1:
                    return this.fixL ? 1 : this.n / 2 + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.m[i];
                case 1:
                    return this.l[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}