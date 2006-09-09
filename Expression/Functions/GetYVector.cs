using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z vektoru bodù vybere souøadnice Y
	/// </summary>
	public class GetYVector: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is PointVector) 
				return (item as PointVector).VectorY;
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Z vektoru bodù (PointVector) vybere souøadnice Y";
		private const string parameters = "PointVector";
	}
}
