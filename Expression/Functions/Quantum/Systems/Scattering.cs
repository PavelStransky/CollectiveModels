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
            this.SetNumParams(5);
            this.SetParam(0, false, true, true, Messages.PTheta, Messages.PThetaDescription, 0.1, typeof(double));
            this.SetParam(1, false, true, true, Messages.PA, Messages.PADescription, -1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PB, Messages.PBDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PC, Messages.PCDescription, 0.5, typeof(double));
            this.SetParam(4, false, true, true, Messages.PD, Messages.PDDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double theta = (double)arguments[0];
            double a = (double)arguments[1];
            double b = (double)arguments[2];
            double c = (double)arguments[3];
            double d = (double)arguments[4];

            return new ComplexScattering(theta, a, b, c, d);
        }

    }
}