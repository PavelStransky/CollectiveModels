using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z argumentù funkce vytvoøí vektor
    /// </summary>
    public class FnVector: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);

            int count = evaluatedArguments.Count;
            for(int i = 0; i < count; i++) {
                this.ConvertInt2Double(evaluatedArguments, i);
                this.CheckArgumentsType(evaluatedArguments, i, typeof(double));
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            Vector result = new Vector(count);

            for(int i = 0; i < count; i++)
                result[i] = (double)arguments[i];

            return result;
        }

        private const string name = "vector";
        private const string help = "Z argumentù funkce vytvoøí vektor (Vector)";
        private const string parameters = "prvky vektoru (int | double)";
    }
}
