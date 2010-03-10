using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n, m
    /// pro bázi lineárního harmonického oscilátoru v polárních souøadnicích
    /// - liché stavy, poèítáme integrací
    /// </summary>
    public class LHOPolarIndexIO: LHOPolarIndexI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOPolarIndexIO(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOPolarIndexIO(Import import) : base(import) { }

        protected override int Parity { get { return 1; } }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn == 0)
                return this.MaxN + 1;
            else
                return this.MaxM / 3 + 1;
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if(qn == 0)
                return this.N[i];
            else
                return this.M[i] / 3;
        }
    }
}
