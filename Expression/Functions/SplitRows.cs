using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z øádkù matice vytvoøí vektory a uspoøádá je do Array
	/// </summary>
	public class SplitRows: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Matrix) {
				Matrix m = item as Matrix;
				Array result = new Array();

				for(int i = 0; i < m.LengthX; i++)
					result.Add(m.GetRowVector(i));
			
				return result;
			}
			else if(item is Array)
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);
		}

		private const string help = "Z øádkù matice vytvoøí vektory a uspoøádá je do Array";
		private const string parameters = "Matrix";
	}
}
