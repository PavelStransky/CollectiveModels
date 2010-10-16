using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the text of the function
    /// </summary>
    public class FunctionText: Fnc {
        public override string Help { get { return Messages.HelpFunctionText; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PFnc, Messages.PFncDescription, null, typeof(UserFunction));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            UserFunction function = (UserFunction)arguments[0];
            return function.Text;
        }
    }
}
