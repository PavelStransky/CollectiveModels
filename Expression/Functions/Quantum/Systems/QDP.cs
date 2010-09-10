using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates Quantum DoublePendulum class
    /// </summary>
    public class QDP: Fnc {
        public override string Help { get { return Messages.HelpQuantumDP; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, false, true, true, Messages.PDoublePendulumMu, Messages.PDoublePendulumMu, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PDoublePendulumLambda, Messages.PDoublePendulumLambdaDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PDoublePendulumGamma, Messages.PDoublePendulumGammaDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PDoublePendulumQuantization, Messages.PDoublePendulumQuantizationDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double mu = (double)arguments[0];
            double lambda = (double)arguments[1];
            double gamma = (double)arguments[2];
            double a = (double)arguments[3];

            return new QuantumDP(mu, lambda, gamma, a);
        }
    }
}