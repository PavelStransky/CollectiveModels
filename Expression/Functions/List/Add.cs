using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Adds an element to the end of the list
    /// </summary>
    public class FnAdd: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpAdd; } }
        public override bool ContextThreadSafe { get { return false; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, true, true, false, Messages.PVariable, Messages.PVariableDescription, null);
            this.SetParam(1, true, true, false, Messages.PItem, Messages.PItemDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = null;

            foreach(object o in arguments) {
                if(result == null)
                    result = o as List;
                else
                    result.Add(o);
            }

            return result;
        }

        private const string name = "add";
    }
}
