using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Sends the request to close the program
	/// </summary>
	public class Exit: FunctionDefinition {
		public override string Help {get {return Messages.HelpExit;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Exit));
			return null;
		}
	}
}
