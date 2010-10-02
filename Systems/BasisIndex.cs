using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// A parrent class for all basis systems
    /// </summary>
    public abstract class BasisIndex: IExportable {
        private Vector basisParams;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="basisParams">Parametry modelu</param>
        public BasisIndex(Vector basisParams) {
            this.Init(basisParams);
        }

        /// <summary>
        /// Inicializuje tøídu
        /// </summary>
        /// <param name="basisParams">Parametry modelu</param>
        protected virtual void Init(Vector basisParams) {
            this.basisParams = basisParams;
        }

        /// <summary>
        /// Celkový poèet prvkù báze
        /// </summary>
        public abstract int Length { get;}

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public abstract int Rank { get;}

        /// <summary>
        /// Index prvku báze s danými kvantovými èísly
        /// </summary>
        /// <param name="basisIndex">Kvantová èísla</param>
        public abstract int this[Vector basisIndex] { get;}

        /// <summary>
        /// Kvantová èísla prvku báze s daným indexem
        /// </summary>
        /// <param name="i">Index báze</param>
        public Vector this[int i] {
            get {
                int rank = this.Rank;

                Vector result = new Vector(rank);
                for(int j = 0; j < rank; j++)
                    result[j] = this.GetBasisQuantumNumber(j, i);

                return result;
            }
        }

        /// <summary>
        /// Vrátí kvantové èíslo
        /// </summary>
        /// <param name="qn">Index kvantového èísla</param>
        /// <param name="i">Poøadí (0...Length)</param>
        public abstract int GetBasisQuantumNumber(int qn, int i);

        /// <summary>
        /// Vrátí maximální index kvantového èísla
        /// </summary>
        /// <param name="qn">Index kvantového èísla</param>
        /// <param name="i">Poøadí (0...Length)</param>
        public abstract int BasisQuantumNumberLength(int qn);

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.basisParams, "Basis Parameters");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public BasisIndex(Import import) {
            IEParam param = new IEParam(import);
            this.Init((Vector)param.Get(null));
        }
        #endregion
    }
}
