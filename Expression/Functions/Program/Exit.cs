using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Sends the request to close the program
	/// </summary>
	public class Exit: Fnc {
		public override string Help {get {return Messages.HelpExit;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Exit));
			return null;
		}
	}
}
