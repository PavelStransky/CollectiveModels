using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z bodu nebo vektoru bod� vybere sou�adnice X
	/// </summary>
	public class GetX: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is PointVector)
                return (item as PointVector).VectorX;
            else if(item is PointD)
                return (item as PointD).X;
            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);			
		}

		private const string help = "Z bodu (PointD) nebo vektoru bod� (PointVector) vybere sou�adnice X";
		private const string parameters = "PointD | PointVector";
	}
}
