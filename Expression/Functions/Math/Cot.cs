using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Cotangent
    /// </summary>
    public class Cot: FncMathD {
        public override string Help { get { return Messages.HelpCot; } }

        protected override double FnDouble(double x, params object[] p) {
            return 1.0 / System.Math.Tan(x);
        }
    }
}
