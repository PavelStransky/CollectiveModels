using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí ClassicalGCM tøídu
    /// </summary>
    public class CGCM : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 4);

            this.ConvertInt2Double(evaluatedArguments, 0);
            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);
            this.ConvertInt2Double(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
        }

        protected virtual object Create(double a, double b, double c, double k) {
            return new ClassicalGCM(a, b, c, k);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            double a = count > 0 ? (double)arguments[0] : -1.0;
            double b = count > 1 ? (double)arguments[1] : 1.0;
            double c = count > 2 ? (double)arguments[2] : 1.0;
            double k = count > 3 ? (double)arguments[3] : 1.0;

            return this.Create(a, b, c, k);
        }

        private const string help = "Vytvoøí tøídu jednoduchého GCM (s jednoduchým kinetickým èlenem)";
        private const string parameters = "[A (double) [; B (double) [; C (double) [; K (double)]]]]";
    }
}