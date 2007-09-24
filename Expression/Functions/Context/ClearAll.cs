using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Delete all variables from the context
    /// </summary>
    public class ClearAll: Fnc {
        public override string Help { get { return Messages.HelpClearAll; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            guider.Context.Clear();
            return guider.Context;
        }
    }
}
