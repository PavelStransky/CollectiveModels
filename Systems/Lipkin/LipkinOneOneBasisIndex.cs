using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy pro Lipkinùv model diagonalizovanı v bázi SU(2) se dvìma individuálními spiny
    /// </summary>
    public class LipkinOneOneBasisIndex: BasisIndex {
        private int n;  // Poèet sdruenıch spinù (extra spin se poèítá zvláš)

        private int[] l, m, s1, s2;
        private bool fixL;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinOneOneBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinOneOneBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.n = (int)basisParams[0];

            // Druhı index: hodnota kvantového èísla l
            if(basisParams.Length > 1) {
                this.fixL = true;

                int maxm = (int)basisParams[1];

                this.l = new int[4 * (maxm + 1)];
                this.m = new int[4 * (maxm + 1)];
                this.s1 = new int[4 * (maxm + 1)];
                this.s2 = new int[4 * (maxm + 1)];

                this.l[0] = maxm;

                int num = 0;
                for(int j = -maxm; j <= maxm; j += 2) {
                    this.l[num] = maxm;
                    this.m[num] = j;
                    this.s1[num] = 0;
                    this.s2[num] = 0;
                    num++;
                    this.l[num] = maxm;
                    this.m[num] = j;
                    this.s1[num] = 0;
                    this.s2[num] = 1;
                    num++;
                    this.l[num] = maxm;
                    this.m[num] = j;
                    this.s1[num] = 1;
                    this.s2[num] = 0;
                    num++;
                    this.l[num] = maxm;
                    this.m[num] = j;
                    this.s1[num] = 1;
                    this.s2[num] = 1;
                    num++;
                }
            }
            else {
                this.fixL = false;

                // Poèet prvkù báze (na toto nejspíš existuje vzoreèák)
                int num = 0;
                for(int i = this.n; i >= 0; i -= 2)
                    for(int j = -i; j <= i; j += 2)
                        num += 2;

                this.l = new int[num];
                this.m = new int[num];
                this.s1 = new int[num];
                this.s2 = new int[num];

                num = 0;
                for(int i = this.n; i >= 0; i -= 2)
                    for(int j = -i; j <= i; j += 2) {
                        this.l[num] = i;
                        this.m[num] = j;
                        this.s1[num] = 0;
                        this.s2[num] = 0;
                        num++;
                        this.l[num] = i;
                        this.m[num] = j;
                        this.s1[num] = 0;
                        this.s2[num] = 1;
                        num++;
                        this.l[num] = i;
                        this.m[num] = j;
                        this.s1[num] = 1;
                        this.s2[num] = 0;
                        num++;
                        this.l[num] = i;
                        this.m[num] = j;
                        this.s1[num] = 1;
                        this.s2[num] = 1;
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
        /// Kvantové èíslo dodateèného spinu 1
        /// </summary>
        public int[] S1 { get { return this.s1; } }

        /// <summary>
        /// Kvantové èíslo dodateèného spinu 2
        /// </summary>
        public int[] S2 { get { return this.s2; } }

        /// <summary>
        /// Poèet sdruenıch bosonù
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return this.fixL ? 3 : 4; } }

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
        public override int BandWidth { get { return 8; } }

        /// <summary>
        /// Maximální hodnota hlavního kvantového èísla l
        /// </summary>
        public int MaxL { get { return this.fixL ? this.l[0] : this.N; } }

        /// <summary>
        /// Maximální hodnota orbitálního kvantového èísla m
        /// </summary>
        public int MaxM { get { return this.N; } }

        /// <summary>
        /// Vrací index prvku s kvantovımi èísly l, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="l">Hlavní kvantové èíslo</param>
        /// <param name="m">Orbitální kvantové èíslo</param>
        public int this[int s1, int s2, int m, int l] {
            get {
                for(int i = 0; i < this.Length; i++)
                    if(this.s1[i] == s1 && this.s2[i] == s2 && this.m[i] == m && this.l[i] == l)
                        return i;
                return -1;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovımi èísly m1, m2.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1], (int)basisIndex[2], (int)basisIndex[3]];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            switch(qn) {
                case 0:
                    return 2;
                case 1:
                    return 2;
                case 2:
                    return this.n + 1;
                case 3:
                    return this.fixL ? 1 : this.n / 2 + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.s1[i];
                case 1:
                    return this.s2[i];
                case 2:
                    return this.m[i];
                case 3:
                    return this.l[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}