using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Øadu vektorù slouèí do øádkù matice
	/// </summary>
	public class JoinRows: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Array));
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			Array array = item as Array;

			if(array.ItemTypeName == typeof(Vector).FullName) {
				if(array.Count == 0)
					return null;

				Matrix m = new Matrix(array.Count, (array[0] as Vector).Length);

				for(int i = 0; i < m.LengthX; i++)
					for(int j = 0; j < m.LengthY; j++)
						m[i, j] = (array[i] as Vector)[j];

				return m;
			}
			else if(array.ItemTypeName == typeof(Array).FullName)
				return this.EvaluateArray(depth, array, arguments);
			else
				return this.BadTypeError(array, 0);
		}

		private const string help = "Øadu vektorù slouèí do øádkù matice";
		private const string parameters = "Array of Vectors";
	}
}
