using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Gets out a variable from the global context
    /// </summary>
    public class GetGlobalVar: FunctionDefinition {
        public override string Help { get { return Messages.HelpGetGlobalVar; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);

            this.SetParam(0, true, false, false, Messages.PVarName, Messages.PVarNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Import import = new Import(Context.GlobalContextFileName, true);
            Context c = import.Read() as Context;
            import.Close();

            return c[arguments[0] as string];
        }
    }
}
