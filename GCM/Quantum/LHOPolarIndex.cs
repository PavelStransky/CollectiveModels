using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje dvojice index� - kvantov�ch ��sel n, m
    /// pro b�zi line�rn�ho harmonick�ho oscil�toru v pol�rn�ch sou�adnic�ch
    /// </summary>
    public class LHOPolarIndex {
        private int[] indexn, indexm;
        private int maxE;

        // Maxim�ln� indexy
        private int maxM, maxN;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie v n�sobc�ch hbar * Omega</param>
        public LHOPolarIndex(int maxE) : this(maxE, false) { }

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="full">V�po�et pro plnou b�zi</param>
        public LHOPolarIndex(int maxE, bool full) : this(maxE, full, 0) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="full">V�po�et pro plnou b�zi</param>
        /// <param name="parity">0...V�echny stavy, 1...Lich� stavy, 2...Sud� stavy</param>
        public LHOPolarIndex(int maxE, bool full, int parity) {
            this.maxE = maxE;

            int mInc = full ? 1 : 3;
            int mBoundMax = ((maxE - 1) / mInc) * mInc;
            int mBoundMin = -mBoundMax;

            if(parity == 1)
                mBoundMin = mInc;
            else if(parity == 2)
                mBoundMax = 0;

            // Zjist�me po�et
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
