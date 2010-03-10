using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// A couple of indexes for the 5D LHO basis calculated via integration
    /// </summary>
    public class LHO5DIndexI: LHO5DIndex {
        private int numSteps;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHO5DIndexI(Vector basisParams) : base(basisParams) {   }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHO5DIndexI(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.numSteps = (int)basisParams[1];

            if(this.numSteps == 0)
                this.numSteps = 10 * this.MaxMu + 1;
        }

        /// <summary>
        /// Po�et krok� pro integraci
        /// </summary>
        public int NumSteps { get { return this.numSteps; } }
    }
}
