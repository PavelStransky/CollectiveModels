using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Sets a variable into the global context
    /// </summary>
    public class SetGlobalVar: FunctionDefinition {
        public override string Help { get { return Messages.SetGlobalVarHelp; } }

        protected override void CreateParameters() {
            this.NumParams(1, true);
            this.SetParam(0, true, false, false, Messages.PVarName, Messages.PVarNameDescription, null);
        }
        
        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fileName = Context.GlobalContextFileName;
            FileInfo fileInfo = new FileInfo(fileName);

            Context c;
            if(fileInfo.Exists) {
                Import import = new Import(Context.GlobalContextFileName, true);
                c = import.Read() as Context;
                import.Close();
            }
            else
                c = new Context();

            int count = arguments.Count;
            for(int i = 0; i < count; i++) {
                Variable v = guider.Context[arguments[i] as string];
                c.SetVariable(v.Name, v.Item);
            }

            Export export = new Export(Context.GlobalContextFileName, true);
            export.Write(c);
            export.Close();

            return c;
        }
    }
}
