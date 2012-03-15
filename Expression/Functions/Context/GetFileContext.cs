using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Core;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the context from a file
    /// </summary>
    public class GetFileContext: Fnc {
        public override string Help { get { return Messages.HelpGetFileContext; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fileName = (string)arguments[0];
            Import import = new Import(fileName);
            if(import.VersionNumber < 9)
                return null;
            
            import.B.ReadString();
            IEParam param = new IEParam(import);           

            object o;
            while(!((o = param.Get()) is Context)) ;                
            import.Close();

            if(o is Context)
                return o;
            else
                return null;
        }
    }
}
