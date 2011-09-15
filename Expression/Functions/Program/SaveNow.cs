using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Saves current document into a file (in current thread)
    /// </summary>
    public class SaveNow: Fnc {
        public override string Help { get { return Messages.HelpSaveNow; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PFileName, Messages.PFileNameDescription, string.Empty, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if((string)arguments[0] != string.Empty)
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.SaveNow, arguments[0] as string));
            else
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.SaveNow));
            return null;
        }
    }
}
