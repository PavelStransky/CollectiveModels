using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vr�t� glob�ln� kontext
    /// </summary>
    public class GetGlobalContext: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 0);

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

        private const string help = "Vr�t� glob�ln� kontext.";
        private const string parameters = "";
    }
}
