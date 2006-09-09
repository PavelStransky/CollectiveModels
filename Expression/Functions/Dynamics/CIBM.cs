using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.IBM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí Classical IBM tøídu
    /// </summary>
    public class CIBM : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.ConvertInt2Double(evaluatedArguments, 0);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                double eta = (double)item;
                double chi = (double)arguments[1];

                return new ClassicalIBM(eta, chi);
            }
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí IBM tøídu pro dané parametry";
        private const string parameters = "eta (double); chi (double)";
    }
}