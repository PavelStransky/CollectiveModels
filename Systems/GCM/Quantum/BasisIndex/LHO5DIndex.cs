using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// A couple of indexes for the 5D LHO basis
    /// </summary>
    public class LHO5DIndex: BasisIndex {
        private int[] indexL, indexMu;
        private int maxE;

        // Maxima of indexes
        private int maxL, maxMu;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHO5DIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHO5DIndex(Import import) : base(import) { }

        /// <summary>
        /// Maximal L for given mu
        /// </summary>
        private int LBoundMax(int mu) {
            return (int)((maxE - 2.5 - 3 * System.Math.Abs(mu)) / 2);
        }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.maxE = (int)basisParams[0];

            int muInc = 1;
            int muBoundMax = ((this.maxE - 3) / 3 / muInc) * muInc;
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

        public override int BasisQuantumNumberLength(int qn) {
            switch(qn) {
                case 0:
                    return this.MaxL + 1;
                case 1:
                    return this.MaxMu + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.L[i];
                case 1:
                    return this.Mu[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        /// <summary>
        /// Maximum energy (in units of hbar * Omega)
        /// </summary>
        public int MaxE { get { return this.maxE; } }

        /// <summary>
        /// Number of elements
        /// </summary>
        public override int Length { get { return this.indexL.Length; } }

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

        /// <summary>
        /// Velikost pásu pásové matice
        /// </summary>
        public virtual int BandWidth { get { return this.maxE; } }
    }
}
