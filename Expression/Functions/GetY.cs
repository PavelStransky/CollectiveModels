using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z bodu nebo vektoru bodù vybere souøadnice Y
	/// </summary>
	public class GetY: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is PointVector)
                return (item as PointVector).VectorY;
            else if(item is PointD)
                return (item as PointD).Y;
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);			
		}

		private const string help = "Z bodu (PointD) nebo vektoru bodù (PointVector) vybere souøadnice Y";
		private const string parameters = "PointD | PointVector";
	}
}
