using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice vybere zadané sloupce
	/// </summary>
	public class GetColumns: FunctionDefinition {
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
				ArrayList columnsToGet = arguments[1] as ArrayList;
				this.CheckIndexesRange(columnsToGet, 0, m.LengthX - 1);

				Matrix result = new Matrix(m.LengthX, columnsToGet.Count);
				for(int i = 0; i < m.LengthX; i++)
					for(int j = 0; j < columnsToGet.Count; j++)
						result[i, j] = m[i, (int)columnsToGet[j]];
				return result;
			}
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);	
		}

		private const string help = "Z matice vybere zadané sloupce";
		private const string parameters = "Matrix; indexy sloupcù (Index)";
	}
}
