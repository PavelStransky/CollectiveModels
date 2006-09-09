using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí polynomiální regresi
	/// </summary>
	public class FnRegression: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is PointVector) {
				PointVector pointVector = item as PointVector;
				int order = (int)arguments[1];
				return Regression.Compute(pointVector, order);
			}
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string name = "regression";
		private const string help = "Vypoèítá koeficienty polynomiální regrese a vrátí je jako vektor.";
		private const string parameters = "PointVector; øád regrese (int)";
	}
}