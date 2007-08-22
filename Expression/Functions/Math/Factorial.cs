using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Factorial
    /// </summary>
    public class Factorial : FncMathD {
        public override string Help { get { return Messages.HelpFactorial; } }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Factorial(x);
        }
    }
}
