using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Hyperbolic Cosine
    /// </summary>
    public class Cosh: FncMathD {
        public override string Help { get { return Messages.HelpCosh; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Cosh(x);
        }
    }
}
