using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Writes a text (or variable) to the writer and begins new line
    /// </summary>
    public class PrintLine : FunctionDefinition {
        public override string Help { get { return Messages.HelpPrintLine; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, false, true, false, Messages.PExpression, Messages.PExpressionDescription, string.Empty);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return guider.WriteLine(arguments[0]);
        }
    }
}
