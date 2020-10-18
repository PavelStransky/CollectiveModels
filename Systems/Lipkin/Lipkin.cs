using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class Lipkin: IExportable {
        // Systém s vlastními hodnotami
        protected EigenSystem eigenSystem;

        // parametry alpha, omega
        protected double alpha, omega;
        protected double alphaIm, omegaIm;

        // Dissipation parameter [ESQPT review 2020, Kopylov & Brandes, New J. Phys. 17, 103031 (2015)]
        protected double kappa;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected Lipkin() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Lipkin(double alpha, double omega) {
            this.alpha = alpha;
            this.omega = omega;
        }

        public Lipkin(double alpha, double omega, double alphaIm, double omegaIm, double kappa)
            : this(alpha, omega) {
            this.alphaIm = alphaIm;
            this.omegaIm = omegaIm;
            this.kappa = kappa;
        }

        public Lipkin(double alpha, double omega, double alphaIm, double omegaIm)
            : this(alpha, omega, alphaIm, omegaIm, 0.0) { }

        #region Implementace IExportable
        /// <summary>
        /// Uložení dodateèných parametrù
        /// </summary>
        protected virtual void Export(IEParam param) { }
        protected virtual void Import(IEParam param) { }

        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.alpha, "Alpha");
            param.Add(this.omega, "Omega");
            param.Add(this.alphaIm, "AlphaIm");
            param.Add(this.omegaIm, "OmegaIm");
            this.Export(param);
            param.Add(this.kappa, "Kappa");

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Lipkin(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.alpha = (double)param.Get(0.0);
            this.omega = (double)param.Get(0.0);
            this.alphaIm = (double)param.Get(0.0);
            this.omegaIm = (double)param.Get(0.0);

            this.Import(param);

            param.Add(this.kappa, "Kappa");
        }

        #endregion
    }
}