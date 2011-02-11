using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n, m
    /// pro bázi lineárního harmonického oscilátoru v polárních souøadnicích
    /// </summary>
    public abstract class LHOPolarIndex: BasisIndex {
        private int[] indexn, indexm;
        private int[] lengthm;              // Poèet n pro jednotlivé m
        private int maxE;

        // Plná báze
        protected bool full = false;

        // Parita
        protected int parity = 0;

        // Maximální indexy
        private int maxM, maxN;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOPolarIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOPolarIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.maxE = (int)basisParams[0];                     // Maximální energie v násobcích hbar * Omega

            int mInc = this.full ? 1 : 3;
            int mBoundMax = ((this.maxE - 1) / mInc) * mInc;
            int mBoundMin = -mBoundMax;

            if(this.parity == 1)
                mBoundMin = mInc;
            else if(this.parity == 2)
                mBoundMin = 0;

            // Zjistíme poèet
            int length = 0;
            int rows = 0;
            for(int m = mBoundMin; m <= mBoundMax; m += mInc) {
                for(int n = 0; n <= (this.maxE - 1 - System.Math.Abs(m)) / 2; n++)
                    length++;
                rows++;
            }

            // Generujeme
            this.indexn = new int[length];
            this.indexm = new int[length];
            this.lengthm = new int[rows];

            int i = 0;
            int j = 0;
            for(int m = mBoundMin; m <= mBoundMax; m += mInc) {
                this.lengthm[j++] = i;
                for(int n = 0; n <= (this.maxE - 1 - System.Math.Abs(m)) / 2; n++) {
                    this.indexm[i] = m;
                    this.indexn[i] = n;
                    i++;
                }
            }

            this.maxM = System.Math.Max(mBoundMax, System.Math.Abs(mBoundMin));
            this.maxN = (this.maxE - 1) / 2;
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m, n.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="n">Index n</param>
        /// <param name="m">Index m</param>
        public int this[int n, int m] {
            get {
                int length = this.Length;
                int llength = this.lengthm.Length;

                for(int i = 0; i < llength; i++) {
                    int j = this.lengthm[i];
                    if(this.indexm[j] == m) {
                        int result = j + n;
                        if(result < length && this.indexm[result] == m)
                            return result;
                        else
                            return -1;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m, n.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Maximální energie (vyjádøená v násobcích hbar * Omega)
        /// </summary>
        public int MaxE { get { return this.maxE; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length { get { return this.indexn.Length; } }

        /// <summary>
        /// Kvantové èíslo N
        /// </summary>
        public int[] N { get { return this.indexn; } }

        /// <summary>
        /// Kvantové èíslo M
        /// </summary>
        public int[] M { get { return this.indexm; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla N
        /// </summary>
        public int MaxN { get { return this.maxN; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla M
        /// </summary>
        public int MaxM { get { return this.maxM; } }

        /// <summary>
        /// Velikost pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return this.maxE - 2; } }
    }
}
