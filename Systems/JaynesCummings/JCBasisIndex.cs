using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje indexy pro Jaynes-Cummings�v model
    /// </summary>
    public class JaynesCummingsBasisIndex: BasisIndex {
        private int j, m2;

        private int[] nb, m;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public JaynesCummingsBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public JaynesCummingsBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
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
        /// Po�et boson�
        /// </summary>
        public int[] Nb { get { return this.nb; } }

        /// <summary>
        /// Orbit�ln� kvantov� ��slo
        /// </summary>
        public int[] M { get { return this.m; } }

        /// <summary>
        /// Druh� integr�l pohybu M2
        /// </summary>
        public int M2 { get { return this.m2; } }

        /// <summary>
        /// Dvakr�t po�et atom�
        /// </summary>
        public int J { get { return this.j; } }

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public override int Rank { get { return 1; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length { get { return this.nb.Length; } }

        /// <summary>
        /// ���ka p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return 3; } }

        /// <summary>
        /// Maxim�ln� hodnota po�tu boson�
        /// </summary>
        public int MaxL { get { return this.nb[this.Length - 1]; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly nb, m
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="nb">Po�et boson�</param>
        /// <param name="m">Orbit�ln� kvantov� ��slo</param>
        public int this[int nb, int m] {
            get {
                for(int i = 0; i < this.Length; i++)
                    if(this.nb[i] == nb && this.m[i] == m)
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