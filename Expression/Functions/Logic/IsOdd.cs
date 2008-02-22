using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// True if the number is odd
    /// </summary>
    public class IsOdd: Fnc {
        public override string Help { get { return Messages.HelpIsOdd; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PInteger, Messages.PIntegerDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return ((int)arguments[0]) % 2 != 0;
        }
    }
}
