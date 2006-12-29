using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Na konec øady pøidá nový prvek
	/// </summary>
	public class AddItem: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(TArray));			
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			return ((item as TArray).Clone() as TArray).Add(arguments[1]);
		}

		private const string help = "Na konec øady pøidá nový prvek";
		private const string parameters = "Array; nový prvek";
	}
}
