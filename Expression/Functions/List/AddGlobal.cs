using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Adds an element to the end of the list in the global context
    /// </summary>
    public class AddGlobal: FDGlobalContext {
        public override string Help { get { return Messages.AddGlobalHelp; } }

        protected override void CreateParameters() {
            this.NumParams(2, true);

            this.SetParam(0, true, false, false, Messages.PVariable, Messages.PVariableDescription, null);
            this.SetParam(1, true, true, false, Messages.PItem, Messages.PItemDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = this.GetGlobalContext();
            object item = c[arguments[0] as string];

            if(item is List) {
                List result = item as List;

                foreach(object o in arguments) {
                    if(result == null)
                        result = item as List;
                    else
                        result.Add(o);
                }

                return result;
            }
            else
                this.BadTypeError(item, 0);

            return null;
        }
    }
}