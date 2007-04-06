using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n, m
    /// pro bázi lineárního harmonického oscilátoru v polárních souøadnicích
    /// </summary>
    public class LHOPolarIndex {
        private int[] indexn, indexm;
        private int maxE;

        // Maximální indexy
        private int maxM, maxN;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxE">Maximální energie v násobcích hbar * Omega</param>
        public LHOPolarIndex(int maxE) : this(maxE, false) { }

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="maxE">Maximální energie v násobcích hbar * Omega</param>
        /// <param name="full">Výpoèet pro plnou bázi</param>
        public LHOPolarIndex(int maxE, bool full) : this(maxE, full, 0) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxE">Maximální energie v násobcích hbar * Omega</param>
        /// <param name="full">Výpoèet pro plnou bázi</param>
        /// <param name="parity">0...Všechny stavy, 1...Liché stavy, 2...Sudé stavy</param>
        public LHOPolarIndex(int maxE, bool full, int parity) {
            this.maxE = maxE;

            int mInc = full ? 1 : 3;
            int mBoundMax = ((maxE - 1) / mInc) * mInc;
            int mBoundMin = -mBoundMax;

            if(parity == 1)
                mBoundMin = mInc;
            else if(parity == 2)
                mBoundMax = 0;

            // Zjistíme poèet
            int length = 0;
            for(int m = mBoundMin; m <= mBoundMax; m += mInc)
                for(int n = 0; n <= (maxE - 1 - System.Math.Abs(m)) / 2; n++)
                    length++;

            // Generujeme
            this.indexn = new int[length];
            this.indexm = new int[length];

            int i = 0;
            for(int m = mBoundMin; m <= mBoundMax; m += mInc)
                for(int n = 0; n <= (maxE - 1 - System.Math.Abs(m)) / 2; n++) {
                    this.indexm[i] = m;
                    this.indexn[i] = n;
                    i++;
                }


            this.maxM = System.Math.Max(mBoundMax, System.Math.Abs(mBoundMin));
            this.maxN = (maxE - 1) / 2;
        }

        /// <summary>
        /// Maximální energie (vyjádøená v násobcích hbar * Omega)
        /// </summary>
        public int MaxE { get { return this.maxE; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public int Length { get { return this.indexn.Length; } }

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
    }
}
