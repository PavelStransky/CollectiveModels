using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje indexy pro Lipkin�v model diagonalizovan� v b�zi SU(2) se dv�ma individu�ln�mi spiny
    /// </summary>
    public class LipkinOneOneBasisIndex: BasisIndex {
        private int n;  // Po�et sdru�en�ch spin� (extra spin se po��t� zvl᚝)

        private int[] l, m, s1, s2;
        private bool fixL;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinOneOneBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinOneOneBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.n = (int)basisParams[0];

            // Druh� index: hodnota kvantov�ho ��sla l
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

                // Po�et prvk� b�ze (na toto nejsp� existuje vzore��k)
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
        /// Hlavn� kvantov� ��slo
        /// </summary>
        public int[] L { get { return this.l; } }

        /// <summary>
        /// Orbit�ln� kvantov� ��slo
        /// </summary>
        public int[] M { get { return this.m; } }

        /// <summary>
        /// Kvantov� ��slo dodate�n�ho spinu 1
        /// </summary>
        public int[] S1 { get { return this.s1; } }

        /// <summary>
        /// Kvantov� ��slo dodate�n�ho spinu 2
        /// </summary>
        public int[] S2 { get { return this.s2; } }

        /// <summary>
        /// Po�et sdru�en�ch boson�
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public override int Rank { get { return this.fixL ? 3 : 4; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length {
            get {
                return this.l.Length;
            }
        }

        /// <summary>
        /// ���ka p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return 8; } }

        /// <summary>
        /// Maxim�ln� hodnota hlavn�ho kvantov�ho ��sla l
        /// </summary>
        public int MaxL { get { return this.fixL ? this.l[0] : this.N; } }

        /// <summary>
        /// Maxim�ln� hodnota orbit�ln�ho kvantov�ho ��sla m
        /// </summary>
        public int MaxM { get { return this.N; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly l, m
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="l">Hlavn� kvantov� ��slo</param>
        /// <param name="m">Orbit�ln� kvantov� ��slo</param>
        public int this[int s1, int s2, int m, int l] {
            get {
                for(int i = 0; i < this.Length; i++)
                    if(this.s1[i] == s1 && this.s2[i] == s2 && this.m[i] == m && this.l[i] == l)
                        return i;
                return -1;
            }
        }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly m1, m2.
        /// Pokud prvek neexistuje, vrac� -1
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