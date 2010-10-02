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
        /// Inicializuje t��du
        /// </summary>
        /// <param name="basisParams">Parametry modelu</param>
        protected virtual void Init(Vector basisParams) {
            this.basisParams = basisParams;
        }

        /// <summary>
        /// Celkov� po�et prvk� b�ze
        /// </summary>
        public abstract int Length { get;}

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public abstract int Rank { get;}

        /// <summary>
        /// Index prvku b�ze s dan�mi kvantov�mi ��sly
        /// </summary>
        /// <param name="basisIndex">Kvantov� ��sla</param>
        public abstract int this[Vector basisIndex] { get;}

        /// <summary>
        /// Kvantov� ��sla prvku b�ze s dan�m indexem
        /// </summary>
        /// <param name="i">Index b�ze</param>
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
        /// Vr�t� kvantov� ��slo
        /// </summary>
        /// <param name="qn">Index kvantov�ho ��sla</param>
        /// <param name="i">Po�ad� (0...Length)</param>
        public abstract int GetBasisQuantumNumber(int qn, int i);

        /// <summary>
        /// Vr�t� maxim�ln� index kvantov�ho ��sla
        /// </summary>
        /// <param name="qn">Index kvantov�ho ��sla</param>
        /// <param name="i">Po�ad� (0...Length)</param>
        public abstract int BasisQuantumNumberLength(int qn);

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.basisParams, "Basis Parameters");
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public BasisIndex(Import import) {
            IEParam param = new IEParam(import);
            this.Init((Vector)param.Get(null));
        }
        #endregion
    }
}
