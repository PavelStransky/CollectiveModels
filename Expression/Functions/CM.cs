using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá korelaèní matici (matice)
	/// </summary>
	public class CM: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(TArray));

			if(evaluatedArguments.Count > 1) 
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			TArray array = item as TArray;

			if(array.ItemTypeName == typeof(Vector).FullName) {
				int shift = (int)arguments[1];

				Matrix result = new Matrix(array.Count);
				for(int i = 0; i < array.Count; i++) {
					for(int j = 0; j <= i; j++) {
						Vector v1 = array[i] as Vector;
						Vector v2 = array[j] as Vector;
						result[i, j] = 0;
						for(int k = 0; k < v1.Length - shift; k++) 
							result[i, j] += (v1[k] * v2[k + shift] + v1[k + shift] * v2[k]) * 0.5;
						result[i, j] /= v1.Length - shift;
						result[j, i] = result[i, j];
					}
				}
				return result;
			}
				
				// Øada øad - pøejdeme o úroveò níž
			else if(array.ItemTypeName == typeof(TArray).FullName) 
				return this.EvaluateArray(depth, array, arguments);
			else
				return this.BadTypeError(array, 0);			
		}

		private const string help = "Vypoèítá korelaèní matici (matice) ze zadané øady vektorù vèetnì zadaného posunu";
		private const string parameters = "Array of Vectors [;posun (int)]";
	}
}
