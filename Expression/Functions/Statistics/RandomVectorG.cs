using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Generates a vector with Gaussian distributed components
    /// </summary>
    public class RandomVectorG: FunctionDefinition {
        private NormalDistribution nd = new NormalDistribution();

        public override string Help { get { return Messages.HelpRandomVectorG; } }

        protected override void CreateParameters() {
            this.NumParams(3);

            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, false, true, true, Messages.PVariance, Messages.PVarianceDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PUpperBound, Messages.PUpperBoundDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            double variance = (double)arguments[1];
            double mean = (double)arguments[2];

            return this.nd.GetVector(length, variance, mean);
        }
    }
}
