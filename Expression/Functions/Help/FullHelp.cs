using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
    /// Full help for the given function (including names and types of the parameters)
	/// Vr�t� n�pov�du k zadan� funkci (v�etn� u�it�)
	/// </summary>
	public class FullHelp: Fnc {
		public override string Help {get {return Messages.HelpFullHelp;}}

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PFnName, Messages.PFnNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return functions[arguments[0] as string].FullHelp;
		}
	}
}
