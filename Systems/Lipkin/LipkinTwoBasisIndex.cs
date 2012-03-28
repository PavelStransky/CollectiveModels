using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy pro Lipkinùv model diagonalizovaný v bázi SU(2), rozdìlený na dvì èásti
    /// </summary>
    public class LipkinTwoBasisIndex: BasisIndex {
        private int n1, n2;  // Poèet sdružených spinù

        private int l1, l2; 

        private int[] m1, m2;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinTwoBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinTwoBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
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
        /// Hlavní kvantové èíslo (systém 1)
        /// </summary>
        public int L1 { get { return this.l1; } }

        /// <summary>
        /// Hlavní kvantové èíslo (systém 2)
        /// </summary>
        public int L2 { get { return this.l2; } }

        /// <summary>
        /// Orbitální kvantové èíslo (systém 1)
        /// </summary>
        public int[] M1 { get { return this.m1; } }

        /// <summary>
        /// Orbitální kvantové èíslo (systém 2)
        /// </summary>
        public int[] M2 { get { return this.m2; } }

        /// <summary>
        /// Poèet bosonù systému 1
        /// </summary>
        public int N1 { get { return this.n1; } }

        /// <summary>
        /// Poèet bosonù systému 2
        /// </summary>
        public int N2 { get { return this.n2; } }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length {
            get {
                return this.m1.Length;
            }
        }

        /// <summary>
        /// Šíøka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.l2 + 2; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly l, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="l">Hlavní kvantové èíslo</param>
        /// <param name="m">Orbitální kvantové èíslo</param>
        public int this[int m1, int m2] {
            get {
                if(m1 < -this.l1 || m1 > this.l1 || m2 < -this.l2 || m2 > this.l2)
                    return -1;

                return ((m1 + this.l1) * (this.l2 + 1) + (m2 + this.l2)) / 2;
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