using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a system to calculate resonances using complex scaling method (Moiseyev)
    /// </summary>
    public class Scattering : Fnc {
        public override string Help { get { return Messages.HelpScattering; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, false, true, true, Messages.PZ, Messages.PZDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PTheta, Messages.PThetaDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double alpha = (double)arguments[0];
            double omega = (double)arguments[1];

            return new ComplexScattering(alpha, omega);
        }

    }
}