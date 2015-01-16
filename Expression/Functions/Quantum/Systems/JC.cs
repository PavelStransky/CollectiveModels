using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Jaynes-Cummings model class
    /// </summary>
    public class JC: Fnc {
        public override string Help { get { return Messages.HelpJC; } }
        public override FncTypes FncType { get { return FncTypes.QuantumSystem; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, false, true, true, Messages.POmega, Messages.POmegaDescription, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega0, Messages.POmega0Description, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PLambda, Messages.PLambdaDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double omega = (double)arguments[0];
            double omega0 = (double)arguments[1];
            double lambda = (double)arguments[2];

            return new JaynesCummings(omega, omega0, lambda);
        }
    }
}