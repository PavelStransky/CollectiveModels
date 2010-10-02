using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n1, n2
    /// pro bázi 2D lineárního harmonického oscilátoru v kartézských souøadnicích
    /// </summary>
    public class LHOCartesianIndex: BasisIndex {
        private int maxn;
        private int numSteps;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOCartesianIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOCartesianIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.maxn = (int)basisParams[0];                     // Maximální energie
            this.numSteps = basisParams.Length > 1 ? (int)basisParams[1] : 0;

            if(this.numSteps == 0)
                this.numSteps = 10 * this.maxn + 1;

        }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length { get { return this.maxn * this.maxn; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n1, n2
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="n1">První kvantové èíslo</param>
        /// <param name="n2">Druhé kvantové èíslo</param>
        public int this[int n1, int n2] {
            get {
                if(n1 >= this.maxn || n1 < 0 || n2 >= this.maxn || n2 < 0)
                    return -1;
                return n1 * this.maxn + n2;
            }
        }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n1, n2.
        /// Pokud prvek neexistuje, vrací -1
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
        /// Poèet krokù pro integraci
        /// </summary>
        public int NumSteps { get { return this.numSteps; } }
    }
}
