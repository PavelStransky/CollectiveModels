using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Sine
    /// </summary>
    public class Sin: FncMathD {
        public override string Help { get { return Messages.HelpSin; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Sin(x);
        }
    }
}
