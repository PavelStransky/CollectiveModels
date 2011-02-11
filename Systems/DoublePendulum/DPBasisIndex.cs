using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje indexy
    /// pro b�zi dvojit�ho kyvadla
    /// </summary>
    public class DPBasisIndex: BasisIndex {
        private int maxm1, maxm2;
        private bool positive;

        private int[] m1, m2;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public DPBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public DPBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.maxm1 = (int)basisParams[0];                     // Maxim�ln� index kyvadla 1
            this.maxm2 = (int)basisParams[1];                     // Maxim�ln� energie kyvadla 2
            this.positive = false;

            if(basisParams.Length > 2 && basisParams[2] > 0)
                this.positive = true;

            int length = this.Length;
            this.m1 = new int[length];
            this.m2 = new int[length];

            int minm1 = this.positive ? 0 : -this.maxm1;

            int k = 0;
            for(int i = minm1; i <= this.maxm1; i++)
                for(int j = -this.maxm2; j <= this.maxm2; j++) {
                    this.m1[k] = i;
                    this.m2[k] = j;
                    k++;
                }
        }

        /// <summary>
        /// Prvn� kvantov� ��slo
        /// </summary>
        public int[] M1 { get { return this.m1; } }

        /// <summary>
        /// Druh� kvantov� ��slo
        /// </summary>
        public int[] M2 { get { return this.m2; } }

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length {
            get {
                if(this.positive)
                    return (this.maxm1 + 1) * (2 * this.maxm2 + 1);
                else
                    return (2 * this.maxm1 + 1) * (2 * this.maxm2 + 1);
            }
        }

        /// <summary>
        /// ���ka p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.maxm2 + 2; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla m1
        /// </summary>
        public int MaxM1 { get { return this.maxm1; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla m1
        /// </summary>
        public int MaxM2 { get { return this.maxm2; } }

        /// <summary>
        /// True, pokud po��t�me jen pozitivn� rotace
        /// </summary>
        public bool Positive { get { return this.positive; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly m1, m2
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="m1">Prvn� kvantov� ��slo</param>
        /// <param name="m2">Druh� kvantov� ��slo</param>
        /// <returns></returns>
        public int this[int m1, int m2] {
            get {
                if(this.positive) {
                    if(m1 < 0 || m1 > this.maxm1 || System.Math.Abs(m2) > this.maxm2)
                        return -1;
                    return m1 * (2 * maxm2 + 1) + maxm2 + m2;
                }
                else {
                    if(System.Math.Abs(m1) > this.maxm1 || System.Math.Abs(m2) > this.maxm2)
                        return -1;
                    return (m1 + maxm1) * (2 * maxm2 + 1) + maxm2 + m2;
                }
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
                    if(this.positive)
                        return this.maxm1 + 1;
                    else
                        return 2 * this.maxm1 + 1;
                case 1:
                    return 2 * this.maxm2 + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.m1[i];
                case 1:
                    return this.m2[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}
