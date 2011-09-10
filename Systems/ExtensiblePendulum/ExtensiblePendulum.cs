using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Extensible pendulum system (2D pendulum on a spring)
    /// </summary>
    public abstract class ExtensiblePendulum: IExportable {
        // Parametr modelu nu = sqrt(mg/kL) = sqrt(mg/k(L0+mg/k))
        private double nu;

        /// <summary>
        /// Parametr modelu Nu = sqrt(mg/kL) = sqrt(mg/k(L0+mg/k))
        /// </summary>
        public double Nu { get { return this.nu; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected ExtensiblePendulum() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="nu">Parametr modelu</param>
        public ExtensiblePendulum(double nu) {
            this.nu = nu;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.nu, "Nu");
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public ExtensiblePendulum(Core.Import import) {
            IEParam param = new IEParam(import);
            this.nu = (double)param.Get(1.0);
        }
        #endregion

        /// <summary>
        /// Vyp�e parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("nu = {0,10:#####0.000}\n", this.nu));
            return s.ToString();
        }
    }
}