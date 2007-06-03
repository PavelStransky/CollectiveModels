using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypo��t� integr�l pod k�ivkou
	/// </summary>
	public class Integrate: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(PointVector));
		}

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return (arguments[0] as PointVector).Integrate();
		}

		private const string help = "Vypo��t� integr�l pod k�ivkou";
		private const string parameters = "PointVector";
	}
}
