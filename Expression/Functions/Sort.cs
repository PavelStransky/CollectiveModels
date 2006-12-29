using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Setøídí øadu / vektor
	/// </summary>
	public class Sort: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			if(evaluatedArguments.Count == 1)
				evaluatedArguments.Add(null);

			// Internì dáme do tøetího argumentu true pro ASC, false pro DESC
			evaluatedArguments.Add(true);

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			object keys = arguments[1];
			bool asc = (bool)arguments[2];

			if(item is ISortable) {
				ISortable s = item as ISortable;

				if(keys == null) {
					if(asc)
						return s.Sort();
					else
						return s.SortDesc();
				}
				else {
					if(keys is Vector) {
						Vector vkeys = keys as Vector;
						if(s.Length != vkeys.Length)
							throw new FunctionDefinitionException(string.Format(errorMessageNotEqualLength, this.Name),
								string.Format(errorMessageNotEqualLengthDetail, s.Length, vkeys.Length, depth));

						if(asc)
							return s.Sort(vkeys);
						else
							return s.SortDesc(vkeys);
					}
					else
						return this.BadTypeError(keys, 1);
				}
			}

			else if(item is TArray) {
				TArray array = item as TArray;

				if(keys == null) {
					if(array.ItemType == typeof(int) || array.ItemType == typeof(double) || array.ItemType == typeof(string)) {
						System.Array a = (System.Array)array;
						System.Array.Sort(a);
						if(!asc)
							System.Array.Reverse(a);
						return (TArray)a;
					}
					else
						return this.EvaluateArray(depth, array, arguments);
				}
				else {
					if(keys is Vector) {
						Vector vkeys = keys as Vector;
						if(asc)
							return (TArray)vkeys.Sort((System.Array) array);
						else
							return (TArray)vkeys.SortDesc((System.Array) array);
					}
					else if(keys is TArray) {
						TArray akeys = keys as TArray;
				
						if(akeys.ItemType == typeof(int) || akeys.ItemType == typeof(double) || akeys.ItemType == typeof(string)) {
							System.Array a = (System.Array)array;
							System.Array k = (System.Array)akeys;
							System.Array.Sort(k, a);
							if(!asc)
								System.Array.Reverse(a);
							return (TArray)a;
						}
						else if(akeys.ItemType == typeof(TArray) || akeys.ItemType == typeof(Vector)) 
							return this.EvaluateArray(depth, array, akeys, arguments);
						else
							return this.BadTypeError(akeys[0], 1);
					}
					else
						return this.BadTypeError(keys, 1);
				}
			}

			else
				return this.BadTypeError(item, 0);
		}

		private const string errorMessageNotEqualLength = "Pro tøídìní pomocí funkce '{0}' je nutné, aby vstupní øada a vektor klíèù mìly stejnou délku.";
		private const string errorMessageNotEqualLengthDetail = "Délka vstupní øady: {0}\nDélka vektoru klíèù: {1}\nHloubka: {2}";

		private const string help = "Setøídí vektor nebo øadu vzestupnì";
		private const string parameters = "Vector | Array of {int | double | string} [;podle èeho tøídit (Vector | Array of {int | double | string})]";
	}
}
