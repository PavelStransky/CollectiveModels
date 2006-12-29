using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice odstraní zadané sloupce
	/// </summary>
	public class DropColumns: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);

			ArrayList newArguments = new ArrayList();
			newArguments.Add(evaluatedArguments[0]);
			newArguments.Add(this.GetIndexArrayFromArgs(evaluatedArguments, 1, false, true));

			return newArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Matrix) {
				Matrix m = item as Matrix;
				ArrayList columnsToDrop = arguments[1] as ArrayList;
				this.CheckIndexesRange(columnsToDrop, 0, m.LengthY - 1);

				Matrix result = new Matrix(m.LengthX, m.LengthY - columnsToDrop.Count);
				int index = 0;
				for(int j = 0; j < m.LengthY; j++)
					if(!columnsToDrop.Contains(j)) {
						for(int i = 0; i < m.LengthX; i++)
							result[i, index] = m[i, j];
						index++;
					}
				return result;
			}
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);
		}

		private const string help = "Z matice odstraní zadané sloupce";
		private const string parameters = "Matrix; indexy sloupcù (Index)";
	}
}
