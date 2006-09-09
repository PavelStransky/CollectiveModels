using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Nastaví interval èasovaèe
	/// </summary>
	public class Interval: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Timer));
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			(item as Timer).Interval = (int)arguments[1];

			return null;
		}

		private const string help = "Nastaví interval èasovaèe";
		private const string parameters = "Timer; interval v ms (int)";
	}
}
