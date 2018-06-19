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
        private int length;

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

            this.length = System.Math.Min(2 * this.j + 1, this.m2 + 1);

            this.m = new int[this.length];
            this.nb = new int[this.length];

            for(int i = 0; i < length; i++) {
                this.m[i] = -this.j + i;
                this.nb[i] = this.m2 - i;
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
        public override int Length { get { return this.length; } }

        /// <summary>
        /// ���ka p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return System.Math.Min(3, this.length); } }

        /// <summary>
        /// Maxim�ln� hodnota po�tu boson�
        /// </summary>
        public int MaxNb { get { return this.m2; } }

        /// <summary>
        /// Minim�ln� hodnota po�tu boson�
        /// </summary>
        public int MinNb { get { return System.Math.Max(0, this.m2 - 2 * this.j); } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly nb, m
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="nb">Po�et boson�</param>
        /// <param name="m">Orbit�ln� kvantov� ��slo</param>
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