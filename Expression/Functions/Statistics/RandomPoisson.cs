using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value with Poisson distribution
    /// </summary>
    public class RandomPoisson: Fnc {
        public override string Help { get { return Messages.HelpRandomPoisson; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return RMTDistribution.GetPoisson();
        }
    }
}
