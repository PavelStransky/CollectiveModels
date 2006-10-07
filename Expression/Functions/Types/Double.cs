using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí hodnoty jako double
    /// </summary>
    public class FnDouble : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }
        public override string Name { get { return name; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is int)
                return (double)(int)item;
            else if(item is double)
                return item;
            else if(item is TimeSpan)
                return ((TimeSpan)item).TotalSeconds;
            else if(item is string)
                return double.Parse(item as string);
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string name = "double";
        private const string help = "Pøevede hodnoty na double";
        private const string parameters = "int | double | TimeSpan | Array";
    }
}
