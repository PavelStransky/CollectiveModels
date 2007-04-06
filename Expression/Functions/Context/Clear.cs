using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vymaže promìnnou z kontextu
	/// </summary>
	public class Clear: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			if(arguments.Count == 0)
				guider.Context.Clear();
			else
				guider.Context.Clear(arguments[0] as string);
			return null;
		}

		private const string help = "Vymaže promìnnou z kontextu";
		private const string parameters = "promìnná (string)";
	}
}
