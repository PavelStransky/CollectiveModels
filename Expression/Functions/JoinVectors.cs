using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Øadu vektorù slouèí do jednoho vektoru
	/// </summary>
	public class JoinVectors: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);

			ArrayList newArguments = new ArrayList();
			if(evaluatedArguments[0] is Array) {
				this.CheckArgumentsMaxNumber(evaluatedArguments, 1);
				newArguments.Add(evaluatedArguments[0]);
			}
			else if(evaluatedArguments[0] is Vector) {
				Array array = new Array(false);
				for(int i = 0; i < evaluatedArguments.Count; i++) {
					this.CheckArgumentsType(evaluatedArguments, i, typeof(Vector));
					array.Add(evaluatedArguments[i]);
				}
				newArguments.Add(array);
			}
			else
				this.BadTypeError(evaluatedArguments[0], 0);

			return newArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			Array array = item as Array;

			if(array.ItemTypeName == typeof(Vector).FullName) {
				int newLength = 0;
				for(int i = 0; i < array.Count; i++) 
					newLength += (array[i] as Vector).Length;

				Vector result = new Vector(newLength);
				int index = 0;
				for(int i = 0; i < array.Count; i++) {
					Vector v = array[i] as Vector;
					for(int j = 0; j < v.Length; j++)
						result[index++] = v[j];
				}

				return result;
			}
			else if(array.ItemTypeName == typeof(Array).FullName) 
				return this.EvaluateArray(depth, array, arguments);
			else
				return this.BadTypeError(item, 0);
		}

		private const string help = "Øadu vektorù nebo vektory slouèí do jednoho vektoru";
		private const string parameters = "Array of Vectors | Vector [;Array of Vectors | Vector [; ...]]";
	}
}
