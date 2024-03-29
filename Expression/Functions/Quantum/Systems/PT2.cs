using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a PT2 class
    /// </summary>
    public class FnPT2: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPT2; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, true, Messages.PMixingParameter, Messages.PMixingParameterDescription, null, typeof(double));
            this.SetParam(1, false, true, true, Messages.PLHOOmega, Messages.PLHOOmegaDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double m = (double)arguments[0];
            double omega = (double)arguments[1];
            double hbar = (double)arguments[2];

            return new PT2(m, omega, hbar);
        }

        private const string name = "pt2";
    }
}