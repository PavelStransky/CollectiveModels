using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrací binomický koeficient
    /// </summary>
    public class BC : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return SpecialFunctions.BinomialCoeficientI((int)arguments[0], (int)arguments[1]);
        }

        private const string help = "Vrací binomické èíslo";
        private const string parameters = "int; int";
    }
}
