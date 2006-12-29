using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí QuantumGCM tøídu
    /// </summary>
    public class QGCM: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 4);

            if(evaluatedArguments[0] is int)
                evaluatedArguments[0] = (double)(int)evaluatedArguments[0];

            if(evaluatedArguments.Count > 1) {
                this.CheckArgumentsNumber(evaluatedArguments, 4);

                this.ConvertInt2Double(evaluatedArguments, 1);
                this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

                this.ConvertInt2Double(evaluatedArguments, 3);
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            }

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                double b = 1;
                double c = 1;
                double k = 1;

                if(arguments.Count > 1) {
                    b = (double)arguments[1];
                    c = (double)arguments[2];
                    k = (double)arguments[3];
                }

                return new QuantumGCM((double)item, b, c, k);
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí GCM tøídu pro dané parametry";
        private const string parameters = "A (double) | Array of A (double); [B (double); C (double); K (double)";
    }
}