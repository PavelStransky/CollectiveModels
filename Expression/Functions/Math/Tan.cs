using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Tangent
    /// </summary>
    public class Tan: FncMathD {
        public override string Help { get { return Messages.HelpTan; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Tan(x);
        }
    }
}
