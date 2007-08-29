using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Delete the specified variable from the context
	/// </summary>
	public class Clear: Fnc {
		public override string Help {get {return Messages.HelpClear;}}

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, false, false, Messages.PVarName, Messages.PVarNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            foreach(string s in arguments)
			    guider.Context.Clear(s);

			return guider.Context;
		}
	}
}
