using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Tries to get the value of the variable from the context; If there the variable
    /// does not exist, returns default value
    /// </summary>
    public class SafeValue : FunctionDefinition {
        public override string Help { get { return Messages.HelpSafeValue; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, false, false, Messages.PVariable, Messages.PVariableDescription, null);
            this.SetParam(1, true, true, false, Messages.PDefaultValue, Messages.PDefaultValueDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string name = arguments[0] as string;
            if(guider.Context.Contains(name))
                return guider.Context[name];
            else
                return arguments[1];
        }
    }
}
