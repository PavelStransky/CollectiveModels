using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Saves current document into a file
    /// </summary>
    public class Save : FunctionDefinition {
        public override string Help { get { return Messages.HelpSave; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PFileName, Messages.PFileNameDescription, string.Empty, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if((string)arguments[0] != string.Empty) 
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Save, arguments[0] as string));
            else
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Save));
            return null;
        }
    }
}
