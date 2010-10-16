using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a user function using the given text
    /// </summary>
    public class FnFunction: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpFunction; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PFncText, Messages.PFncTextDescription, null, typeof(string));
            this.SetParam(1, false, true, false, Messages.PVariable, Messages.PVariableDescription, string.Empty, typeof(string));
            this.SetParam(2, false, true, false, Messages.PContext, Messages.PContextDescription, null, typeof(Context));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string text = (string)arguments[0];
            string retVariable = (string)arguments[1];
            Context context = (Context)arguments[2];
            return new UserFunction(text, context, retVariable);
        }

        private const string name = "function";
    }
}
