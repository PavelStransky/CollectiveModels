using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí hodnoty jako int
    /// </summary>
    public class FnInt : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }
        public override string Name { get { return name; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is int)
                return item;
            else if(item is double)
                return (int)(double)item;
            else if(item is TimeSpan)
                return (int)((TimeSpan)item).TotalSeconds;
            else if(item is string)
                return int.Parse(item as string);
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string name = "int";
        private const string help = "Pøevede hodnoty na int";
        private const string parameters = "int | double | TimeSpan | Array";
    }
}
