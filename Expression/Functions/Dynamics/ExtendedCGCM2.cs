using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí ExtendedClassicalGCM2 tøídu
    /// </summary>
    public class ExtendedCGCM2 : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 5);

            this.ConvertInt2Double(evaluatedArguments, 0);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

            this.ConvertInt2Double(evaluatedArguments, 3);
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));

            this.ConvertInt2Double(evaluatedArguments, 4);
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                double b = (double)arguments[1];
                double c = (double)arguments[2];
                double k = (double)arguments[3];
                double lambda = (double)arguments[4];

                return new ExtendedClassicalGCM2((double)item, b, c, k, lambda);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí tøídu rozšíøeného GCM (s kinetickým èlenem úmìrným beta^2) pro dané parametry";
        private const string parameters = "A (double) | Array of A (double); B (double); C (double); K (double); lambda (double)";
    }
}