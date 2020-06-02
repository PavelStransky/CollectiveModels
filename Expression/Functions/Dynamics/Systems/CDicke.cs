using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a classical Dicke class
    /// </summary>
    public class CDicke : Fnc {
        public override string Help { get { return Messages.HelpClassicalDicke; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);
            this.SetParam(0, false, true, true, Messages.POmega0, Messages.POmega0Description, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega, Messages.POmegaDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PGamma, Messages.PGammaDetail, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PJ, Messages.PJDescription, 1.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PDelta, Messages.PDeltaDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double omega0 = (double)arguments[0];
            double omega = (double)arguments[1];
            double gamma = (double)arguments[2];
            double j = (double)arguments[3];
            double delta = (double)arguments[4];

            return new ClassicalDicke(omega0, omega, gamma, j, delta, 1.0);
        }
    }
}