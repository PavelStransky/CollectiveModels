using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje dvojice index� - kvantov�ch ��sel n1, n2
    /// pro b�zi 2D line�rn�ho harmonick�ho oscil�toru v kart�zsk�ch sou�adnic�ch
    /// </summary>
    public class LHOCartesianIndex: BasisIndex {
        private int maxn;
        private int numSteps;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOCartesianIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOCartesianIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.maxn = (int)basisParams[0];                     // Maxim�ln� energie
            this.numSteps = basisParams.Length > 1 ? (int)basisParams[1] : 0;

            if(this.numSteps == 0)
                this.numSteps = 10 * this.maxn + 1;

        }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length { get { return this.maxn * this.maxn; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly n1, n2
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="n1">Prvn� kvantov� ��slo</param>
        /// <param name="n2">Druh� kvantov� ��slo</param>
        public int this[int n1, int n2] {
            get {
                if(n1 >= this.maxn || n1 < 0 || n2 >= this.maxn || n2 < 0)
                    return -1;
                return n1 * this.maxn + n2;
            }
        }

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly n1, n2.
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn == 0 || qn == 1)
                return this.maxn;

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return i / this.maxn;
                case 1:
                    return i % this.maxn;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        /// <summary>
        /// Po�et krok� pro integraci
        /// </summary>
        public int NumSteps { get { return this.numSteps; } }
    }
}
