using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Hyperbolic Sine
    /// </summary>
    public class Sinh: FncMathD {
        public override string Help { get { return Messages.HelpSinh; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Sinh(x);
        }
    }
}
