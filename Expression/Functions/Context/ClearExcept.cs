using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Erase all variables except specified ones from the context
    /// </summary>
    public class ClearExcept: Fnc {
        public override string Help { get { return Messages.HelpClearExcept; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, false, false, false, Messages.PVarName, Messages.PVarNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ArrayList a = new ArrayList();

            foreach(string s in arguments)
                a.Add(guider.Context[s] as Variable);

            guider.Context.Clear();

            foreach(Variable v in a)
                guider.Context.SetVariable(v.Name, v.Item);

            return guider.Context;
        }
    }
}
