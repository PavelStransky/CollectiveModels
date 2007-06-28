using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Deletes specified variable (all variables) from the global context
    /// </summary>
    public class ClearGlobal: FDGlobalContext {
        public override string Help { get { return Messages.ClearGlobalHelp; } }

        protected override void CreateParameters() {
            this.NumParams(1, true);
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
