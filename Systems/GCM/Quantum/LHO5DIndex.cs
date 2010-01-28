using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    /// <summary>
    /// A couple of indexes for the 5D LHO basis
    /// </summary>
    public class LHO5DIndex {
        private int[] indexL, indexMu;
        private int maxE;

        // Maxima of indexes
        private int maxL, maxMu;

        /// <summary>
        /// Maximal L for given mu
        /// </summary>
        private int LBoundMax(int mu) {
            return (int)((maxE - 2.5 - 3 * System.Math.Abs(mu)) / 2);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxE">Maximum energy in hbar * Omega</param>
        public LHO5DIndex(int maxE) {
            this.maxE = maxE;

            int muInc = 1;
            int muBoundMax = ((maxE - 3) / 3 / muInc) * muInc;
            int muBoundMin = 0;

            // Finding out the number of indexes
            int length = 0;
            for(int mu = muBoundMin; mu <= muBoundMax; mu += muInc)
                for(int l = 0; l <= this.LBoundMax(mu); l++)
                    length++;

            // Generating indexes
            this.indexL = new int[length];
            this.indexMu = new int[length];

            int i = 0;
            for(int mu = muBoundMin; mu <= muBoundMax; mu += muInc)
                for(int l = 0; l <= this.LBoundMax(mu); l++) {
                    this.indexMu[i] = mu;
                    this.indexL[i] = l;
                    i++;
                }


            this.maxMu = muBoundMax;
            this.maxL = this.LBoundMax(0);
        }

        /// <summary>
        /// Maximum energy (in units of hbar * Omega)
        /// </summary>
        public int MaxE { get { return this.maxE; } }

        /// <summary>
        /// Number of elements
        /// </summary>
        public int Length { get { return this.indexL.Length; } }

        /// <summary>
        /// Quantum number l
        /// </summary>
        public int[] L { get { return this.indexL; } }

        /// <summary>
        /// Quantum number mu
        /// </summary>
        public int[] Mu { get { return this.indexMu; } }

        /// <summary>
        /// Maximum value of quantum number l
        /// </summary>
        public int MaxL { get { return this.maxL; } }

        /// <summary>
        /// Maximum value of quantum number mu
        /// </summary>
        public int MaxMu { get { return this.maxMu; } }
    }
}
