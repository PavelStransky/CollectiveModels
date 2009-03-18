using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a list of all parameters
    /// </summary>
    public class FnList: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpList; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, false, true, false, Messages.PItem, Messages.PItemDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = new List();

            foreach(object o in arguments)
                result.Add(o);

            return result;
        }

        private const string name = "list";
    }
}
