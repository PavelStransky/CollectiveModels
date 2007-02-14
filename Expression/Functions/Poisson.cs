using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá hodnotu Poissonovy distribuèní funkce v daném bodì
    /// </summary>
    public class Poisson: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is int)
                return SpecialFunctions.Poisson((int)item);
            if(item is double)
                return SpecialFunctions.Poisson((double)item);
            else if(item is Vector)
                return (item as Vector).Transform(SpecialFunctions.Poisson);
            else if(item is Matrix)
                return (item as Matrix).Transform(SpecialFunctions.Poisson);
            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vypoèítá hodnotu Poissonovy distribuèní funkce v daném bodì";
        private const string parameters = "int | double | Vector | Matrix";
    }
}
