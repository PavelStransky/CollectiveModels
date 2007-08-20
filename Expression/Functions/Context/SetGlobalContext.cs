using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Sets a new global context
    /// </summary>
    public class SetGlobalContext: FDGlobalContext {
        public override string Help { get { return Messages.HelpSetGlobalContext; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PContext, Messages.PContextDescription, null, typeof(Context));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = arguments[0] as Context;
            this.SetGlobalContext(c);
            return c;
        }
    }
}
