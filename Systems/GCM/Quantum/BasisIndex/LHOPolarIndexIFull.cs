using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje dvojice index� - kvantov�ch ��sel n, m
    /// pro b�zi line�rn�ho harmonick�ho oscil�toru v pol�rn�ch sou�adnic�ch
    /// - v�echny stavy, po��t�me integrac�
    /// </summary>
    public class LHOPolarIndexIFull: LHOPolarIndexI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public LHOPolarIndexIFull(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro na�ten� dat
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
        /// Velikost p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.MaxE - 2; } }
    }
}
