using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Double pendulum system
    /// </summary>
    /// <remarks>Physical Review E 70, 066218 (2004)</remarks>
    public abstract class DoublePendulum: IExportable {
        // Parametry modelu (redukovaná hmotnost, redukovaná délka)
        private double mu, lambda;

        // Gravitaèní parametr
        private double gamma;

        /// <summary>
        /// Parametr Mu (redukovaná hmotnost m2 / m1)
        /// </summary>
        public double Mu { get { return this.mu; } }

        /// <summary>
        /// Parametr Lambda (redukovaná délka l2 / l1)
        /// </summary>
        public double Lambda { get { return this.lambda; } }
        
        /// <summary>
        /// Gravitaèní parametr g * m1 * l1
        /// </summary>
        public double Gamma { get { return this.gamma; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected DoublePendulum() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Pomìr hmotností tìles</param>
        /// <param name="lambda">Pomìr délek</param>
        /// <param name="gamma">Gravitaèní parametr</param>
        public DoublePendulum(double mu, double lambda, double gamma) {
            this.mu = mu;
            this.lambda = lambda;
            this.gamma = gamma;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.mu, "Mu");
            param.Add(this.lambda, "Lambda");
            param.Add(this.gamma, "Gamma");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public DoublePendulum(Core.Import import) {
            IEParam param = new IEParam(import);

            this.mu = (double)param.Get(1.0);
            this.lambda = (double)param.Get(1.0);
            this.gamma = (double)param.Get(1.0);
        }    
        #endregion

        /// <summary>
        /// Vypíše parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("mu = {0,10:#####0.000}\nlambda = {1,10:#####0.000}\ngamma = {2,10:#####0.000}\n", this.mu, this.lambda, this.gamma));
            return s.ToString();
        }
    }
}