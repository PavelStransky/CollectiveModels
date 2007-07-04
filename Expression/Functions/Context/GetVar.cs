using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Returns a variable from a given context
    /// </summary>
    public class GetVar : FunctionDefinition {
        public override string Help { get { return Messages.HelpGetVar; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.PContext, Messages.PContextDescription, null, typeof(Context));
            this.SetParam(1, true, false, false, Messages.PVarName, Messages.PVarNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = arguments[0] as Context;
            string name = arguments[1] as string;
            return c[name];
        }
    }
}
