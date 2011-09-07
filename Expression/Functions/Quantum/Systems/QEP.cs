using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates Quantum Extensible Pendulum class
    /// </summary>
    public class QEP: Fnc {
        public override string Help { get { return Messages.HelpQuantumEP; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, false, true, true, Messages.PDoublePendulumMu, Messages.PDoublePendulumMu, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PA0, Messages.PA0Description, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double mu = (double)arguments[0];
            double a = (double)arguments[1];
            double hbar = (double)arguments[2];

            return new QuantumEP(mu, a, hbar);
        }
    }
}