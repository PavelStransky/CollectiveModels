using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of Poisson distribution
    /// </summary>
    public class Poisson: FncMathD {
        public override string Help { get { return Messages.HelpPoisson; } }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Poisson(x);
        }
    }
}
