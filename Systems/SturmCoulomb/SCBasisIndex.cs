using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje indexy
    /// pro b�zi Sturm-Coulombova probl�mu
    /// </summary>
    public class SCBasisIndex: BasisIndex {
        private int maxn, maxl;

        private int[] n, l;

        // Kvantov� ��slo m (projekce �hlov�ho momentu)
        private int m;

        // Energie vlastn�ch stav�
        private Vector e;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SCBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public SCBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.maxn = (int)basisParams[0];                     
            this.maxl = (int)basisParams[1];
            this.m = (int)basisParams[2];

            int length = this.maxn * this.maxl;
            this.n = new int[length];
            this.l = new int[length];

            int k = 0;
            for(int i = 0; i < this.maxl; i++)
                for(int j = 0; j < this.maxn; j++) {
                    this.n[k] = j;
                    this.l[k] = i;
                    k++;
                }

        }

        /// <summary>
        /// Hlavn� kvantov� ��slo
        /// </summary>
        public int[] N { get { return this.n; } }

        /// <summary>
        /// Moment hybnosti
        /// </summary>
        public int[] L { get { return this.l; } }

        /// <summary>
        /// Projekce momentu hybnosti
        /// </summary>
        public int M { get { return this.m; } }

        /// <summary>
        /// Po�et kvantov�ch ��sel
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length { get { return this.l.Length; } }

        /// <summary>
        /// ���ka p�su matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.maxn + 1; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla l
        /// </summary>
        public int MaxL { get { return this.maxl; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla n
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly n, l, m
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="n">Hlavn� kvantov� ��slo</param>
        /// <param name="l">�hlov� moment</param>
        public int this[int n, int l] {
            get {
                return n * this.maxl + l;
            }
        }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly n, l
        /// Pokud prvek neexistuje, vrac� -1
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
                    return this.maxn;
                case 1:
                    return this.maxl;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.n[i];
                case 1:
                    return this.l[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}
