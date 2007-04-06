using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Nastaví nový globální kontext
    /// </summary>
    public class SetContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Context));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = arguments[0] as Context;
            guider.Context.OnEvent(new ContextEventArgs(ContextEventType.SetContext, c));
            return c;
        }

        private const string help = "Nastaví nový globální kontext";
        private const string parameters = "Kontext";
    }
}