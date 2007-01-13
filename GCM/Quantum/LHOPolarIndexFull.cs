using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n, m
    /// pro bázi lineárního harmonického oscilátoru v polárních souøadnicích
    /// </summary>
    public class LHOPolarIndexFull {
        private int[] indexn, indexm;
        private int maxE;

        // Maximální indexy
        private int maxM, maxN;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxE">Maximální energie v násobcích hbar * Omega</param>
        public LHOPolarIndexFull(int maxE) {
            this.maxE = maxE;

            // Zjistíme poèet
            int length = 0;
            for(int n = 0; n <= (maxE - 1) / 2; n++)
                for(int m = -(maxE - 1 - 2 * n); m <= maxE - 1 - 2 * n; m++)
                    length++;

            // Generujeme
            this.indexn = new int[length];
            this.indexm = new int[length];

            int i = 0;
            for(int n = 0; n <= (maxE - 1) / 2; n++) {
                for(int m = -(maxE - 1 - 2 * n); m <= maxE - 1 - 2 * n; m++) {
                    this.maxM = System.Math.Max(this.maxM, m);
                    this.indexm[i] = m;
                    this.indexn[i] = n;
                    i++;
                }
            }

            this.maxN = this.indexn[length - 1];
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
