using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypíše text (promìnnou) do writeru
    /// </summary>
    public class FnPrint : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPrint; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, false, true, false, Messages.PExpression, Messages.PExpressionDescription, string.Empty);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return guider.WriteLine(arguments[0]);
        }

        private const string name = "print";
    }
}
