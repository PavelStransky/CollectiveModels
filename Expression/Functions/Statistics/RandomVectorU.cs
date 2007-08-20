using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Generates a vector with uniformly distributed components
    /// </summary>
    public class RandomVectorU: FunctionDefinition {
        private Random random = new Random();

        public override string Help { get { return Messages.HelpRandomVectorU; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, false, true, true, Messages.PLowerBound, Messages.PLowerBoundDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PUpperBound, Messages.PUpperBoundDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            double min = (double)arguments[1];
            double max = (double)arguments[2];

            double d = max - min;

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = this.random.NextDouble() * d + min;

            return result;
        }
    }
}
