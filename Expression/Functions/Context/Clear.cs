using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Delete the specified variable (all variables) from the context
	/// </summary>
	public class Clear: Fnc {
		public override string Help {get {return Messages.HelpClear;}}

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, false, false, false, Messages.PVarName, Messages.PVarNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] == null)
				guider.Context.Clear();
			else
                foreach(string s in arguments)
				    guider.Context.Clear(s);

			return null;
		}
	}
}
