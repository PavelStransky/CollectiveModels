using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates uniformly distributed random numbers between given limits
    /// </summary>
    public class RandomU : Fnc {
        private Random random = new Random();

        public override string Help { get { return Messages.HelpRandomU; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, false, true, true, Messages.PLowerBound, Messages.PLowerBoundDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PUpperBound, Messages.PUpperBoundDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double min = (double)arguments[0];
            double max = (double)arguments[1];

            return this.random.NextDouble() * (max - min) + min;
        }
    }
}
