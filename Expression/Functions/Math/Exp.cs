using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Exponential
    /// </summary>
    public class Exp: FncMathD {
        public override string Help { get { return Messages.HelpExp; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Exp(x);
        }
    }
}
