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

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.ConvertInt2Double(evaluatedArguments, 0);
            this.ConvertInt2Double(evaluatedArguments, 1);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
                double eta = (double)arguments[0];
                double chi = (double)arguments[1];

                return new ClassicalIBM(eta, chi);
        }

        private const string help = "Vytvoøí IBM tøídu";
        private const string parameters = "eta (double); chi (double)";
    }
}