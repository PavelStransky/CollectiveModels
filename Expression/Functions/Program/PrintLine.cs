using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypíše text (promìnnou) do writeru
    /// </summary>
    public class PrintLine : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 1);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments.Count > 0)
                return guider.WriteLine(arguments[0]);
            else
                return guider.WriteLine();
        }

        private const string name = "printline";
        private const string help = "Vypíše výsledek výrazu a zalomí øádku";
        private const string parameters = "[výraz]";
    }
}
