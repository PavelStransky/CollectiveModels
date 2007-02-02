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

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            ArrayList evaluatedArguments = this.EvaluateArguments(context, arguments, writer);
			this.CheckArgumentsNumber(evaluatedArguments, 0);			
			context.OnEvent(new ContextEventArgs(ContextEventType.Exit));

			return null;
		}

		private const string help = "Vy�le po�adavek o ukon�en� programu";
	}
}
