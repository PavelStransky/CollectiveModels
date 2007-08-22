using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns global context
    /// </summary>
    public class GetGlobalContext: FncGlobalContext {
        public override string Help { get { return Messages.HelpGetGlobalContext; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return this.GetGlobalContext();
        }
    }
}
