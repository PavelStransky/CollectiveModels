using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Returns global context
    /// </summary>
    public class GetGlobalContext: FunctionDefinition {
        public override string Help { get { return Messages.HelpGetGlobalContext; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fileName = Context.GlobalContextFileName;
            FileInfo fileInfo = new FileInfo(fileName);

            Context result;
            if(fileInfo.Exists) {
                Import import = new Import(Context.GlobalContextFileName, true);
                result = import.Read() as Context;
                import.Close();
            }
            else
                result = new Context();

            return result;
        }
    }
}
