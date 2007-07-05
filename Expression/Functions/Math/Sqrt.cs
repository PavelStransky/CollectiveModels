using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Square root
    /// </summary>
    public class Sqrt: MathFnD {
        public override string Help { get { return Messages.HelpSqrt; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Sqrt(x);
        }
    }
}
