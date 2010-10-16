using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Evaluates a user function
    /// </summary>
    public class Evaluate: Fnc {
        public override string Help { get { return Messages.HelpEvaluate; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);
            this.SetParam(0, true, true, false, Messages.PFnc, Messages.PFncDescription, null, typeof(UserFunction));
            this.SetParam(1, false, true, false, Messages.PParam, Messages.PParamDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            UserFunction function = (UserFunction)arguments[0];

            arguments.RemoveAt(0);
            if(arguments[0] == null)
                arguments.RemoveAt(0);

            return function.Evaluate(arguments, guider);
        }
    }
}
