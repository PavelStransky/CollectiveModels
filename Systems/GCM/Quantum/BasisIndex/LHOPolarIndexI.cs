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
    public class LHOPolarIndexI: LHOPolarIndex {
        private int numSteps;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOPolarIndexI(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOPolarIndexI(Import import) : base(import) { }

        /// <summary>
        /// Parita vlnových funkcí
        /// </summary>
        protected virtual int Parity { get { return 0; } }

        /// <summary>
        /// Plná báze
        /// </summary>
        protected virtual bool Full { get { return false; } }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            this.parity = Parity;
            this.full = Full;

            base.Init(basisParams);

            this.numSteps = basisParams.Length > 1 ? (int)basisParams[1] : 0;

            if(this.numSteps == 0)
                this.numSteps = 10 * this.MaxM + 1;
        }

        public override int BasisQuantumNumberLength(int qn) {
            switch(qn) {
                case 0:
                    return this.MaxN + 1;
                case 1:
                    return this.MaxM / 3 * 2 + 3;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.N[i];
                case 1:
                    return (this.M[i] + this.MaxM) / 3;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        /// <summary>
        /// Poèet krokù pro integraci
        /// </summary>
        public int NumSteps { get { return this.numSteps; } }
    }
}
