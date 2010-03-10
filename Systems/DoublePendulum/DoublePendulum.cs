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
        // Parametry modelu (redukovan� hmotnost, redukovan� d�lka)
        private double mu, lambda;

        // Gravita�n� parametr
        private double gamma;

        /// <summary>
        /// Parametr Mu (redukovan� hmotnost m2 / m1)
        /// </summary>
        public double Mu { get { return this.mu; } }

        /// <summary>
        /// Parametr Lambda (redukovan� d�lka l2 / l1)
        /// </summary>
        public double Lambda { get { return this.lambda; } }
        
        /// <summary>
        /// Gravita�n� parametr g * m1 * l1
        /// </summary>
        public double Gamma { get { return this.gamma; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected DoublePendulum() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Pom�r hmotnost� t�les</param>
        /// <param name="lambda">Pom�r d�lek</param>
        /// <param name="gamma">Gravita�n� parametr</param>
        public DoublePendulum(double mu, double lambda, double gamma) {
            this.mu = mu;
            this.lambda = lambda;
            this.gamma = gamma;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� v�sledky do souboru
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
        /// Na�te v�sledky ze souboru
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
        /// Vyp�e parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("mu = {0,10:#####0.000}\nlambda = {1,10:#####0.000}\ngamma = {2,10:#####0.000}\n", this.mu, this.lambda, this.gamma));
            return s.ToString();
        }
    }
}