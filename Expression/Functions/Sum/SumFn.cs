using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for all sum functions
    /// </summary>
    public class SumFn: FunctionDefinition {
        public override string Parameters { get { return Messages.SumParameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, true,
                typeof(Vector), typeof(double), typeof(int), typeof(Matrix));
        }
    }
}
