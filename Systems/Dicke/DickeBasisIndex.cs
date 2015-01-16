using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Třída, která v sobě zapouzdřuje dvojice indexů - kvantových čísel n, m
    /// pro bázi lineárního harmonického oscilátoru v kartézských souřadnicích
    /// </summary>
    public class DickeBasisIndex : BasisIndex {
        private int j, maxN;              // Nejvyšší M, N
        private int mod;
        private int length;

        private int[] indexn;
        private int[] indexm;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public DickeBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro načtení dat
        /// </summary>
        /// <param name="import">Import</param>
        public DickeBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.maxN = (int)basisParams[0];
            this.j = (int)basisParams[1];

            this.length = (this.maxN + 1) * (2 * this.j + 1);
            this.mod = (2 * this.j + 1);

            this.indexm = new int[this.length];
            this.indexn = new int[this.length];

            int i = 0;
            for(int n = 0; n <= this.maxN; n++)
                for(int m = -this.j; m <= this.j; m++) {
                    this.indexn[i] = n;
                    this.indexm[i] = m;
                    i++;
                }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými čísly m, n.
        /// </summary>
        /// <param name="n">Index n</param>
        /// <param name="m">Index m</param>
        public int this[int n, int m] {
            get {
                if(n > this.maxN || n < 0 || m < -this.j || m > this.j)
                    return -1;
                return n * mod + m + this.j;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými čísly m, n.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        /// <summary>
        /// Počet indexů báze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Počet prvků
        /// </summary>
        public override int Length { get { return this.length; } }

        /// <summary>
        /// Kvantové číslo N
        /// </summary>
        public int[] N { get { return this.indexn; } }

        /// <summary>
        /// Kvantové číslo M
        /// </summary>
        public int[] M { get { return this.indexm; } }

        /// <summary>
        /// Maximální hodnota kvantového čísla N
        /// </summary>
        public int MaxN { get { return this.maxN; } }

        /// <summary>
        /// Maximální hodnota kvantového čísla M
        /// </summary>
        public int J { get { return this.j; } }

        /// <summary>
        /// Velikost pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return this.mod + 1; } }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn == 0)
                return this.MaxN + 1;
            else
                return 2 * this.J + 1;
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if(qn == 0)
                return this.N[i];
            else
                return this.M[i];
        }
    }
}
