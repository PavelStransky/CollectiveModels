using System;
using System.IO;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for functions that needs global context
    /// </summary>
    public class FncGlobalContext: Fnc {
        protected Context GetGlobalContext() {
            string fileName = Context.GlobalContextFileName;
            FileInfo fileInfo = new FileInfo(fileName);

            Context c;
            if(fileInfo.Exists) {
                Import import = new Import(fileName);
                import.SetVersionNumber(import.B.ReadInt32());
                c = import.Read() as Context;
                import.Close();
            }
            else
                c = new Context();

            return c;
        }

        protected void SetGlobalContext(Context c) {
            Export export = new Export(Context.GlobalContextFileName, IETypes.Binary, 7);
            export.Write(c);
            export.Close();
        }
    }
}
