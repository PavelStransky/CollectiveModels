using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Deletes specified variable (all variables) from the global context
    /// </summary>
    public class ClearGlobal: FncGlobalContext {
        public override string Help { get { return Messages.HelpClearGlobal; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, false, false, Messages.PVarName, Messages.PVarNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = this.GetGlobalContext();

            foreach(string s in arguments)
                c.Clear(s);

            this.SetGlobalContext(c);
            return c;
        }
    }
}
