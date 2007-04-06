using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pøedek k vytváøení potomkù LHOQuantumGCM tøídy
    /// </summary>
    public abstract class LHOQGCM: FunctionDefinition {
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 6);

            this.ConvertInt2Double(evaluatedArguments, 0);
            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);
            this.ConvertInt2Double(evaluatedArguments, 3);
            this.ConvertInt2Double(evaluatedArguments, 4);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 5, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            double a = count > 0 ? (double)arguments[0] : -1.0;
            double b = count > 1 ? (double)arguments[1] : 1.0;
            double c = count > 2 ? (double)arguments[2] : 1.0;
            double k = count > 3 ? (double)arguments[3] : 1.0;
            double a0 = count > 4 ? (double)arguments[4] : 1.0;
            double hbar = count > 5 ? (double)arguments[5] : 0.01;

            return this.Create(a, b, c, k, a0, hbar);
        }

        protected abstract object Create(double a, double b, double c, double k, double a0, double hbar);

        private const string parameters = "[A (double)[; B (double)[; C (double)[; K (double)[; A0 (double)[; hbar (double)]]]]]]";
    }
}