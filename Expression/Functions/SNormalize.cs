using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Normalizuje data staticticky
	/// </summary>
	public class SNormalize: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);			
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Vector)
				return (item as Vector).StatisticalNormalization();
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Normalizuje vektor (statisticky, mean = 0, var = 1)";
		private const string parameters = "Vector";
	}
}
