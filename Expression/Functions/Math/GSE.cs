using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of Wigner GSE distribution
    /// </summary>
    public class GSE: FncMathD {
        public override string Help { get { return Messages.HelpGSE; } }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.GSE(x);
        }
    }
}
