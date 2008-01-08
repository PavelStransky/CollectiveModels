using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Cosine
    /// </summary>
    public class Cos: FncMathD {
        public override string Help { get { return Messages.HelpCos; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Cos(x);
        }
    }
}
