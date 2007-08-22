using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vypíše text (promìnnou) do writeru
    /// </summary>
    public class FnPrint : Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPrint; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PExpression, Messages.PExpressionDescription, string.Empty);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return guider.Write(arguments[0]);
        }

        private const string name = "print";
    }
}
