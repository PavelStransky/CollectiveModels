using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje dvojice index� - kvantov�ch ��sel n, m
    /// pro b�zi line�rn�ho harmonick�ho oscil�toru v pol�rn�ch sou�adnic�ch
    /// </summary>
    public class LHOPolarIndexFull {
        private int[] indexn, indexm;
        private int maxE;

        // Maxim�ln� indexy
        private int maxM, maxN;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie v n�sobc�ch hbar * Omega</param>
        public LHOPolarIndexFull(int maxE) {
            this.maxE = maxE;

            // Zjist�me po�et
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
        /// Maxim�ln� energie (vyj�d�en� v n�sobc�ch hbar * Omega)
        /// </summary>
        public int MaxE { get { return this.maxE; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public int Length { get { return this.indexn.Length; } }

        /// <summary>
        /// Kvantov� ��slo N
        /// </summary>
        public int[] N { get { return this.indexn; } }

        /// <summary>
        /// Kvantov� ��slo M
        /// </summary>
        public int[] M { get { return this.indexm; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla N
        /// </summary>
        public int MaxN { get { return this.maxN; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla M
        /// </summary>
        public int MaxM { get { return this.maxM; } }
    }
}
