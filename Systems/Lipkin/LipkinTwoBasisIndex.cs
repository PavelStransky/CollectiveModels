using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje indexy pro Lipkin�v model diagonalizovan� v b�zi SU(2), rozd�len� na dv� ��sti
    /// </summary>
    public class LipkinTwoBasisIndex: BasisIndex {
        private int n1, n2;  // Po�et sdru�en�ch spin�

        private int l1, l2; 

        private int[] m1, m2;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinTwoBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinTwoBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.n1 = (int)basisParams[0];
            this.l1 = (int)basisParams[1];
            this.n2 = (int)basisParams[2];
            this.l2 = (int)basisParams[3];

            int num = (this.l1 + 1) * (this.l2 + 1);

            this.m1 = new int[num];
            this.m2 = new int[num];

            num = 0;
            for(int i = -this.l1; i <= this.l1; i += 2)
                for(int j = -this.l2; j <= this.l2; j += 2) {
                    this.m1[num] = i;
                    this.m2[num] = j;
                    num++;
                }
        }

        /// <summary>
        /// Hlavn� kvantov� ��slo (syst�m 1)
        /// </summary>
        public int L1 { get { return this.l1; } }

        /// <summary>
        /// Hlavn� kvantov� ��slo (syst�m 2)
        /// </summary>
        public int L2 { get { return this.l2; } }

        /// <summary>
        /// Orbit�ln� kvantov� ��slo (syst�m 1)
        /// </summary>
        public int[] M1 { get { return this.m1; } }

        /// <summary>
        /// Orbit�ln� kvantov� ��slo (syst�m 2)
        /// </summary>
        public int[] M2 { get { return this.m2; } }

        /// <summary>
        /// Po�et boson� syst�mu 1
        /// </summary>
        public int N1 { get { return this.n1; } }

        /// <summary>
        /// Po�et boson� syst�mu 2
        /// </summary>
        public int N2 { get { return this.n2; } }

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length {
            get {
                return this.m1.Length;
            }
        }

        /// <summary>
        /// ���ka p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.l2 + 2; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly l, m
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="l">Hlavn� kvantov� ��slo</param>
        /// <param name="m">Orbit�ln� kvantov� ��slo</param>
        public int this[int m1, int m2] {
            get {
                if(m1 < -this.l1 || m1 > this.l1 || m2 < -this.l2 || m2 > this.l2)
                    return -1;

                return ((m1 + this.l1) * (this.l2 + 1) + (m2 + this.l2)) / 2;
            }
        }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly m1, m2.
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
                    return this.l1 + 1;
                case 1:
                    return this.l2 + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 1:
                    return this.m1[i];
                case 2:
                    return this.m2[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}