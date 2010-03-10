using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates classical DoublePendulum class
    /// </summary>
    public class CDP: Fnc {
        public override string Help { get { return Messages.HelpClassicalDP; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, false, true, true, Messages.PDoublePendulumMu, Messages.PDoublePendulumMu, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PDoublePendulumLambda, Messages.PDoublePendulumLambdaDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PDoublePendulumGamma, Messages.PDoublePendulumGammaDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double mu = (double)arguments[0];
            double lambda = (double)arguments[1];
            double gamma = (double)arguments[2];

            return new ClassicalDP(mu, lambda, gamma);
        }
    }
}