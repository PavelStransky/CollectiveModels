using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Delete the specified variable (all variables) from the context
	/// </summary>
	public class Clear: FunctionDefinition {
		public override string Help {get {return Messages.ClearHelp;}}

        protected override void CreateParameters() {
            this.NumParams(1, true);
            this.SetParam(0, false, false, false, Messages.PVarName, Messages.PVarNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments.Count == 0)
				guider.Context.Clear();
			else
                foreach(string s in arguments)
				    guider.Context.Clear(s);

			return null;
		}
	}
}
