using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Gamma
    /// </summary>
    public class Gamma: FncMathD {
        public override string Help { get { return Messages.HelpGamma; } }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Exp(SpecialFunctions.GammaLog(x));
        }
    }
}
