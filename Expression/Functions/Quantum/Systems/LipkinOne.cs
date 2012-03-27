using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a Lipkin model class diagonalized in the full SU(2) basis + one single spin
    /// </summary>
    public class LipkinOneFnc: Fnc {
        public override string Help { get { return Messages.HelpLipkinF; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, false, true, true, Messages.PAlpha, Messages.PAlphaDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega, Messages.POmegaDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double alpha = (double)arguments[0];
            double omega = (double)arguments[1];

            return new LipkinOne(alpha, omega);
        }

        const string name = "lipkinone";
    }
}