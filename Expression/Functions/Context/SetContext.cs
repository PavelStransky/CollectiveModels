using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Sets a new context
    /// </summary>
    public class SetContext : FunctionDefinition {
        public override string Help { get { return Messages.HelpSetContext; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.PContext, Messages.PContextDescription, null, typeof(Context));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = arguments[0] as Context;
            guider.Context.OnEvent(new ContextEventArgs(ContextEventType.SetContext, c));
            return c;
        }
    }
}