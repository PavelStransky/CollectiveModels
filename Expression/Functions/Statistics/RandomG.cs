using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Generates Gaussian distributed random numbers with given variance and mean
    /// </summary>
    public class RandomG: FunctionDefinition {
        private NormalDistribution nd = new NormalDistribution();

        public override string Help { get { return Messages.HelpRandomG; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, false, true, true, Messages.PVariance, Messages.PVarianceDescription, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PUpperBound, Messages.PUpperBoundDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double variance = (double)arguments[0];
            double mean = (double)arguments[1];

            return this.nd.GetValue(variance, mean);
        }
    }
}
