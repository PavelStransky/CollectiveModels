using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates the quantum Dicke class
    /// </summary>
    public class QDicke : Fnc {
        public override string Help { get { return Messages.HelpQuantumDicke; } }

        protected override void CreateParameters() {
            this.SetNumParams(8);
            this.SetParam(0, false, true, true, Messages.POmega0, Messages.POmega0Description, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega, Messages.POmegaDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PGamma, Messages.PGammaDetail, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PJ, Messages.PJDescription, 1.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PDelta, Messages.PDeltaDescription, 1.0, typeof(double));
            this.SetParam(5, false, true, true, Messages.PXi, Messages.PXiDescription, 1.0, typeof(double));
            this.SetParam(6, false, true, true, Messages.PKappa, Messages.PKappaDescription, 0.0, typeof(double));
            this.SetParam(7, false, true, true, Messages.PMu, Messages.PMuDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double omega0 = (double)arguments[0];
            double omega = (double)arguments[1];
            double gamma = (double)arguments[2];
            double j = (double)arguments[3];
            double delta = (double)arguments[4];
            double xi = (double)arguments[5];
            double kappa = (double)arguments[6];
            double mu = (double)arguments[7];

            return new QuantumDicke(omega0, omega, gamma, j, delta, xi, kappa, mu);
        }
    }
}