using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje dvojice index� - kvantov�ch ��sel n, m
    /// pro b�zi line�rn�ho harmonick�ho oscil�toru v pol�rn�ch sou�adnic�ch
    /// - lich� stavy
    /// </summary>
    public class LHOPolarIndexO: LHOPolarIndex {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOPolarIndexO(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public LHOPolarIndexO(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            this.parity = 1;
            this.full = false;

            base.Init(basisParams);
        }

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