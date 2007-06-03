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

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(int), typeof(double), typeof(string), typeof(TimeSpan));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is double)
                return (int)(double)item;
            else if(item is TimeSpan)
                return (int)((TimeSpan)item).TotalSeconds;
            else if(item is string)
                return int.Parse(item as string);
            else
                return item;
        }

        private const string name = "int";
        private const string help = "Pøevede hodnoty na int";
        private const string parameters = "int | double | TimeSpan | string";
    }
}
