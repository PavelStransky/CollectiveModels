using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Hyperbolic Tangent
    /// </summary>
    public class Tanh: FncMathD {
        public override string Help { get { return Messages.HelpTanh; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Tanh(x);
        }
    }
}
