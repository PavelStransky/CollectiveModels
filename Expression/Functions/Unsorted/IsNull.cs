using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// True if a given expression is null
    /// </summary>
    public class IsNull: Fnc {
        public override string Help { get { return Messages.HelpIsNull; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PValue, Messages.PValueDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] == null)
                return true;
            return false;
        }
    }
}
