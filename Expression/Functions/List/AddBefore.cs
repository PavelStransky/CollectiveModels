using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Adds an element to the beginning of the list
    /// </summary>
    public class AddBefore: Fnc {
        public override string Help { get { return Messages.HelpAddBefore; } }
        public override bool ContextThreadSafe { get { return false; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, true, true, false, Messages.PVariable, Messages.PVariableDescription, null);
            this.SetParam(1, true, true, false, Messages.PItem, Messages.PItemDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = arguments[0] as List;

            int count = arguments.Count;

            for(int i = count - 1; i > 0; i--)
                result.Insert(0, arguments[i]);

            return result;
        }
    }
}
