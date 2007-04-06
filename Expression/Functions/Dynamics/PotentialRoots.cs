using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá koøeny rovnice potenciál == energie
    /// </summary>
    public class PotentialRoots : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 3);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(PavelStransky.GCM.GCM));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PavelStransky.GCM.GCM gcm = arguments[0] as PavelStransky.GCM.GCM;
            double e = (double)arguments[1];
            double gamma = (double)arguments[2];
            return (item as PavelStransky.GCM.GCM).Roots(e, gamma);
        }

        private const string help = "Vypoèítá koøeny rovnice potenciál == energie pro zadané gamma";
        private const string parameters = "GCM; energie (double); gamma (double)";
    }
}
