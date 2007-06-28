using System;
using System.IO;
using System.Text;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for functions that needs global context
    /// </summary>
    public class FDGlobalContext: FunctionDefinition {
        protected Context GetGlobalContext() {
            string fileName = Context.GlobalContextFileName;
            FileInfo fileInfo = new FileInfo(fileName);

            Context c;
            if(fileInfo.Exists) {
                Import import = new Import(fileName, true);
                c = import.Read() as Context;
                import.Close();
            }
            else
                c = new Context();

            return c;
        }

        protected void SetGlobalContext(Context c) {
            Export export = new Export(Context.GlobalContextFileName, true);
            export.Write(c);
            export.Close();
        }
    }
}
