using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vy�le po�adavek o ukon�en� programu
	/// </summary>
	public class Exit: FunctionDefinition {
		public override string Help {get {return help;}}

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 0);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Exit));
			return null;
		}

		private const string help = "Vy�le po�adavek o ukon�en� programu";
	}
}
