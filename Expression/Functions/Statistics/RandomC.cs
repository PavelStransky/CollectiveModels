using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a random number with semicircle distribution between -R and R
    /// </summary>
    public class RandomC : Fnc {
        private Random random = new Random();

        public override string Help { get { return Messages.HelpRandomC; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);

            this.SetParam(0, false, true, true, Messages.PRadius, Messages.PRadiusDescription, 1, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double r = (double)arguments[0];
            return RMTDistribution.GetSemicircle(r);
        }
    }
}
