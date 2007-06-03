using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Value of Poisson distribution
    /// </summary>
    public class Poisson: MathFnD {
        public override string Help { get { return Messages.PoissonHelp; } }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Poisson(x);
        }
    }
}
