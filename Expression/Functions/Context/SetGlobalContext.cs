using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Nastaví nový globální kontext
    /// </summary>
    public class SetGlobalContext: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Context));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fileName = Context.GlobalContextFileName;

            Export export = new Export(Context.GlobalContextFileName, true);
            export.Write(arguments[0]);
            export.Close();

            return arguments[0];
        }

        private const string help = "Nastaví nový globální kontext";
        private const string parameters = "Kontext";
    }
}
