using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Zastaví èasovaè
	/// </summary>
	public class Stop: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Timer));			
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			(item as Timer).Enabled = false;

			return null;
		}

		private const string help = "Zastaví èasovaè";
		private const string parameters = "Timer";	
	}
}
