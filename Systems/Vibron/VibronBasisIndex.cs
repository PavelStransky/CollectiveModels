using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems
{
    /// <summary>
    /// Třída, která v sobě zapouzdřuje indexy pro vibronový U(4) model diagonalizovaný v bázi U(3)
    /// </summary>
    public class VibronBasisIndex : BasisIndex
    {
        private int n, l;
        private int[] np;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public VibronBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro načtení dat
        /// </summary>
        /// <param name="import">Import</param>
        public VibronBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.n = (int)basisParams[0];
            this.l = (int)basisParams[1];

            this.np = new int[(this.n - this.l) / 2 + 1];


            for (int i = this.l; i <= this.n; i += 2)
                this.np[(i - this.l) / 2] = i;        }

        /// <summary>
        /// Hlavní kvantové číslo
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Moment hybnosti
        /// </summary>
        public int L { get { return this.l; } }

        /// <summary>
        /// Počet p bosonů
        /// </summary>
        public int[] Np { get { return this.np; } }

        /// <summary>
        /// Počet indexů báze
        /// </summary>
        public override int Rank { get { return 1; } }

        /// <summary>
        /// Počet prvků
        /// </summary>
        public override int Length { get { return this.np.Length; } }

        /// <summary>
        /// Šířka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return 3; } }

        /// <summary>
        /// Vrací index prvku s kvantovými čísly l, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="np">Počet p bosonů</param>
        public int this[int np]
        {
            get
            {
                for (int i = 0; i < this.Length; i++)
                    if (this.np[i] == np)
                        return i;
                return -1;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými čísly m1, m2.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex]
        {
            get
            {
                return this[(int)basisIndex[0]];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            switch (qn) {
                case 0:
                    return this.np.Length;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch (qn) {
                case 0:
                    return this.np[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}