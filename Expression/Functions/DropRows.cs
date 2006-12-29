using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice odstran� zadan� ��dky
	/// </summary>
	public class DropRows: FunctionDefinition {
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
				ArrayList rowsToDrop = arguments[1] as ArrayList;
				this.CheckIndexesRange(rowsToDrop, 0, m.LengthX - 1);

				Matrix result = new Matrix(m.LengthX - rowsToDrop.Count, m.LengthY);
				int index = 0;
				for(int i = 0; i < m.LengthX; i++)
					if(!rowsToDrop.Contains(i)) {
						for(int j = 0; j < m.LengthY; j++)
							result[index, j] = m[i, j];
						index++;
					}
				return result;
			}
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Z matice odstran� zadan� ��dky";
		private const string parameters = "Matrix; indexy ��dk� (Index)";
	}
}
