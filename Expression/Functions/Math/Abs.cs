using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací absolutní hodnotu objektu v argumentu (po prvcích)
	/// </summary>
	public class Abs: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is int)
				return System.Math.Abs((int)item);
			else if(item is double)
				return System.Math.Abs((double)item);
			else if(item is PointD)
				return new PointD((item as PointD).X, System.Math.Abs((item as PointD).Y));
			else if(item is Vector)
				return (item as Vector).Transform(new RealFunction(System.Math.Abs));
			else if(item is PointVector)
				return (item as PointVector).Transform(null, new RealFunction(System.Math.Abs));
			else if(item is Matrix) 
				return (item as Matrix).Transform(new RealFunction(System.Math.Abs));
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací absolutní hodnotu objektu v argumentu (po prvcích), u bodu nebo vektoru bodù dìlá absolutní hodnotu pouze z Y složky";
		private const string parameters = "int | double | Point | Vector | PointVector | Matrix";
	}
}
