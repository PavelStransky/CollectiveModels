using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí PointVector
	/// </summary>
	public class FnPointVector: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			if(evaluatedArguments.Count == 1)
				this.CheckArgumentsType(evaluatedArguments, 0, typeof(Array));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			// Øada bodù
			if(arguments.Count == 1) {
				Array array = item as Array;

				if(array.ItemTypeName == typeof(Array).FullName)
					return this.EvaluateArray(depth, array, arguments);
				else if(array.ItemTypeName == typeof(PointD).FullName) {
					PointVector result = new PointVector(array.Count);

					for(int i = 0; i < result.Length; i++)
						result[i] = (PointD)array[i];

					return result;
				}
				else
					return this.BadTypeError(array[0], 0);
			}
			// Dva vektory
			else {
				if(item is Array) {
					if(arguments[1] is Array) 
						return this.EvaluateArray(depth, item as Array, arguments[1] as Array, arguments);
					else
						return this.BadTypeError(arguments[1], 1);
				}
				else if(item is Vector) {
					if(arguments[1] is Vector) {
						Vector v1 = item as Vector;
						Vector v2 = arguments[1] as Vector;

						if(v1.Length != v2.Length)
							throw new FunctionDefinitionException(string.Format(errorMessageNotEqualLengthVector, this.Name),
								string.Format(errorMessageNotEqualLengthVectorDetail, v1.Length, v2.Length, depth));

						PointVector result = new PointVector(v1.Length);
						for(int i = 0; i < result.Length; i++)
							result[i] = new PointD(v1[i], v2[i]);

						return result;
					}
					else
						return this.BadTypeError(arguments[1], 1);
				}
				else
					return this.BadTypeError(item, 0);
			}
		}

		private const string errorMessageNotEqualLengthVector = "Pro vytvoøení øady vektorù pomocí funkce '{0}' je nutné, aby vstupní vektory mìly stejnou délku.";
		private const string errorMessageNotEqualLengthVectorDetail = "Délka prvního vektoru: {0}\nDélka druhého vektoru: {1}\nHloubka: {2}";

		private const string name = "pointvector";
		private const string help = "Vytvoøí vektor bodù (PointVector)";
		private const string parameters = "(Vector x; Vector y) | Array of PointD";
	}
}
