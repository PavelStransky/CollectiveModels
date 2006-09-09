using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Prohodí dimenze v hloubkách h1, h2
	/// </summary>
	public class SwapDim: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 3);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Array));
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
			this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));

			if((int)evaluatedArguments[1] > (int)evaluatedArguments[2]) {
				int i = (int)evaluatedArguments[1];
				evaluatedArguments[1] = (int)evaluatedArguments[2];
				evaluatedArguments[2] = i;
			}

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Array) {
				Array array = item as Array;
				int h1 = (int)arguments[1];			// h1 < h2
				int h2 = (int)arguments[2];
				if(h1 == h2)
					return item;

				if(h1 > depth)
					return this.EvaluateArray(depth, array, arguments);
				
				// h1 == depth
				Array a = array;
				ArrayList lengths = new ArrayList();
				lengths.Add(a.Count);
				for(int i = h1; i < h2; i++) {
					if(a.Count > 0 && a.ItemTypeName == typeof(Array).FullName) {
						a = a[0] as Array;
						lengths.Add(a.Count);
					}
					else
						this.BadTypeError(a, 0);
				}

				int length = a.Count;
				ArrayList index = lengths.Clone() as ArrayList;
				Array result = new Array();
				for(int j = 0; j < length; j++) {
					index[lengths.Count - 1] = j;
					result.Add(this.SetItem(1, array, index, lengths));
				}
			
				return result;
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private object GetItem(int depth, Array array, ArrayList index) {
			if(depth < index.Count - 1)
				return this.GetItem(depth + 1, array[(int)index[depth]] as Array, index);
			else
				return array[(int)index[depth]];
		}

		private Array SetItem(int depth, Array array, ArrayList index, ArrayList lengths) {
			Array result = new Array();

			if(depth < lengths.Count - 1) {
				int length = (int)lengths[depth];
				for(int i = 0; i < length; i++) {
					index[depth] = i;
					result.Add(this.SetItem(depth + 1, array, index, lengths));
				}
			}
			else {
				int length = (int)lengths[0];
				for(int i = 0; i < length; i++) {
					index[0] = i;
					result.Add(this.GetItem(0, array, index));
				}
			}

			return result;
		}

		private const string help = "Prohodí dimenze objektu v zadaných hloubkách. Objekty v zadaných hloubkách musí být typu Array";
		private const string parameters = "Array; hloubka1 (int); hloubka2 (int)";
	}
}
