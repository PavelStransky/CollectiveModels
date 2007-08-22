using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns actual context
    /// </summary>
    public class GetContext : Fnc {
        public override string Help { get { return Messages.HelpGetContext; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return guider.Context;
        }
    }
}
