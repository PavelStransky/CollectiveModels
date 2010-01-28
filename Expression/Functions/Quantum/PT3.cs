using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a PT3 class
    /// </summary>
    public class FnPT3: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPT3; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, true, Messages.PA, Messages.PADescription, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.PB, Messages.PBDescription, null, typeof(double));
            this.SetParam(2, false, true, true, Messages.PLHOOmega, Messages.PLHOOmegaDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double omega = (double)arguments[2];
            double hbar = (double)arguments[3];

            return new PT3(a, b, omega, hbar);
        }

        private const string name = "pt3";
    }
}