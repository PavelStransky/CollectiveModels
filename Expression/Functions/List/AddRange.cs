using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Adds a range of elements to the the list
    /// </summary>
    public class AddRange : Fnc {
        public override string Help { get { return Messages.HelpAddRange; } }
        public override bool ContextThreadSafe { get { return false; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, true, true, false, Messages.PVariable, Messages.PVariableDescription, null);
            this.SetParam(1, true, true, false, Messages.PItem, Messages.PItemDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = arguments[0] as List;

            int count = arguments.Count;

            for (int i = 1; i < count; i++) {
                if (arguments[i] is Vector)
                    result.AddRange((arguments[i] as Vector).GetItems());
                else if (arguments[i] is TArray)
                    result.AddRange((arguments[i] as TArray).GetItems());
            }
                                     
            return result;
        }
    }
}
