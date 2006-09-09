using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí GCM tøídu
    /// </summary>
    public class FnGCM: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 5);

            if(evaluatedArguments[0] is int)
                evaluatedArguments[0] = (double)(int)evaluatedArguments[0];

            if(evaluatedArguments.Count > 1) {
                this.CheckArgumentsMinNumber(evaluatedArguments, 4);

                this.ConvertInt2Double(evaluatedArguments, 1);
                this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

                this.ConvertInt2Double(evaluatedArguments, 3);
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            }

            if(evaluatedArguments.Count > 4) {
                this.ConvertInt2Double(evaluatedArguments, 4);
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));
            }

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                double b = 1;
                double c = 1;
                double k = 1;
                double l = 0;

                if(arguments.Count > 1) {
                    b = (double)arguments[1];
                    c = (double)arguments[2];
                    k = (double)arguments[3];
                }

                if(arguments.Count > 4)
                    l = (double)arguments[4];

                return new ClassicalGCM((double)item, b, c, k, l);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string name = "gcm";
        private const string help = "Vytvoøí GCM tøídu pro dané parametry";
        private const string parameters = "A (double) | Array of A (double); [B (double); C (double); K (double)[; L (double)]]";
    }
}