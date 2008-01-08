using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Hyperbolic Cotangent
    /// </summary>
    public class Coth: FncMathD {
        public override string Help { get { return Messages.HelpCoth; } }

        protected override double FnDouble(double x, params object[] p) {
            return 1.0 / System.Math.Tanh(x);
        }
    }
}
