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

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 0);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(double), typeof(TimeSpan), typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is TimeSpan)
                return ((TimeSpan)item).TotalSeconds;
            else if(item is string)
                return double.Parse(item as string);
            else
                return item;
        }

        private const string name = "double";
        private const string help = "Pøevede hodnoty na double";
        private const string parameters = "int | double | TimeSpan";
    }
}
