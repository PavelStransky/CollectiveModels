using System;
using System.IO;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Sets a variable into the global context
    /// </summary>
    public class SetGlobalVar: FDGlobalContext {
        public override string Help { get { return Messages.SetGlobalVarHelp; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, false, false, Messages.PVarName, Messages.PVarNameDescription, null);
            this.SetParam(1, false, true, false, Messages.PValue, Messages.PValueDescription, null);
        }
        
        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = this.GetGlobalContext();

            Variable v = (arguments.Count > 1) ? new Variable(arguments[0] as string, arguments[1]) : guider.Context[arguments[0] as string];
            c.SetVariable(v.Name, v.Item);

            this.SetGlobalContext(c);

            return c;
        }
    }
}
