using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n, m
    /// pro bázi lineárního harmonického oscilátoru v polárních souøadnicích
    /// - všechny stavy, poèítáme integrací
    /// </summary>
    public class LHOPolarIndexIFull: LHOPolarIndexI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOPolarIndexIFull(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOPolarIndexIFull(Import import) : base(import) { }

        protected override bool Full { get { return true; } }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn == 0)
                return this.MaxN + 1;
            else
                return this.MaxM * 2 + 3;
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if(qn == 0)
                return this.N[i];
            else
                return this.M[i] + this.MaxM;
        }

        /// <summary>
        /// Velikost pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.MaxE - 2; } }
    }
}
