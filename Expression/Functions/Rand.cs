using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrac� n�hodn� ��slo v zadan�m intervalu
    /// </summary>
    public class FnRand : FunctionDefinition {
        private Random random = new Random();

        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }
        public override string Name { get { return name; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.ConvertInt2Double(evaluatedArguments, 0);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));
            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            double min = (double)arguments[0];
            double max = (double)arguments[1];

            return this.random.NextDouble() * (max - min) + min;
        }

        private const string name = "rand";
        private const string help = "Vrac� n�hodn� ��slo v zadan�m intervalu";
        private const string parameters = "minim�ln� mez (double); maxim�ln� mez (double)";
    }
}
