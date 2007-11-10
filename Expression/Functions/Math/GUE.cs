using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of Wigner GUE distribution
    /// </summary>
    public class GUE: FncMathD {
        public override string Help { get { return Messages.HelpGUE; } }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.GUE(x);
        }
    }
}
