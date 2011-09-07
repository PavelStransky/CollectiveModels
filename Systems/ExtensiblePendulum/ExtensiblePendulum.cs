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
        // Parametr modelu Mu = sqrt(mg/kL) = sqrt(mg/k(L0+mg/k))
        private double mu;

        /// <summary>
        /// Parametr modelu Mu = sqrt(mg/kL) = sqrt(mg/k(L0+mg/k))
        /// </summary>
        public double Mu { get { return this.mu; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected ExtensiblePendulum() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Parametr modelu</param>
        public ExtensiblePendulum(double mu) {
            this.mu = mu;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.mu, "Mu");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public ExtensiblePendulum(Core.Import import) {
            IEParam param = new IEParam(import);
            this.mu = (double)param.Get(1.0);
        }
        #endregion

        /// <summary>
        /// Vypíše parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("mu = {0,10:#####0.000}\n", this.mu));
            return s.ToString();
        }
    }
}