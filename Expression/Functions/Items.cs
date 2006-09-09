using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z øady v prvním argumentu vybere prvky z hloubky podle druhého argumentu 
	/// a podle indexù v tøetím a následujících parametrech
	/// </summary>
	public class Items: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Use {get {return use;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 3);
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

			ArrayList newArguments = new ArrayList();
			newArguments.Add(evaluatedArguments[0]);
			newArguments.Add(evaluatedArguments[1]);
			newArguments.Add(this.GetIndexArrayFromArgs(evaluatedArguments, 2, true, false));

			return newArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			ArrayList indexes = arguments[0] as ArrayList;
			int requiredDepth = (int)(arguments[1]);

			if(depth < requiredDepth) {
				if(item is PavelStransky.Expression.Array)
					return this.EvaluateArray(depth, item as PavelStransky.Expression.Array, arguments);
				else
					return this.BadTypeError(item, 0);
			}
			else {
				if(item is PavelStransky.Math.Vector) {
					PavelStransky.Math.Vector v = item as PavelStransky.Math.Vector;
					this.CheckIndexesRange(indexes, 0, v.Length - 1);

					PavelStransky.Math.Vector result = new PavelStransky.Math.Vector(indexes.Count);
					for(int i = 0; i < result.Length; i++)
						result[i] = v[(int)indexes[i]];
						
					return result;
				}
				else if(item is PavelStransky.Expression.Array) {
					PavelStransky.Expression.Array a = item as PavelStransky.Expression.Array;
					this.CheckIndexesRange(indexes, 0, a.Count - 1);

					PavelStransky.Expression.Array result = new PavelStransky.Expression.Array();
					for(int i = 0; i < indexes.Count; i++)
						result.Add(a[(int)indexes[i]]);

					return result;
				}
				else
					return this.BadTypeError(item, 0);
			}
		}

		private const string help = "Z øady v prvním argumentu vybere prvky z hloubky podle druhého argumentu a podle indexù v tøetím a následujících parametrech";
		private const string use = "items(Array | Vector, hloubka, indexy...)";
	}
}
