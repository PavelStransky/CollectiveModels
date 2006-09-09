using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice vybere zadané øádky
	/// </summary>
	public class GetRows: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			ArrayList newArguments = new ArrayList();
			newArguments.Add(evaluatedArguments[0]);
			newArguments.Add(this.GetIndexArrayFromArgs(evaluatedArguments, 1, true, false));

			return newArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Matrix) {
				Matrix m = item as Matrix;
				ArrayList rowsToGet = arguments[1] as ArrayList;
				this.CheckIndexesRange(rowsToGet, 0, m.LengthX - 1);

				Matrix result = new Matrix(rowsToGet.Count, m.LengthY);
				for(int i = 0; i < rowsToGet.Count; i++)
					for(int j = 0; j < m.LengthY; j++)
						result[i, j] = m[(int)rowsToGet[i], j];
				return result;
			}
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Z matice vybere zadané øádky";
		private const string parameters = "Matrix; indexy øádkù (Index)";
	}
}
