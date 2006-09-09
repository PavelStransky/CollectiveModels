using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací index prvku s nejnižší èíselnou hodnotou
	/// </summary>
	public class MinIndex: FunctionDefinitionMinMaxIndex {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector) {
				Vector v = item as Vector;
				int i = v.MinIndex();

				Array index = new Array();
				index.Add(i);

				return this.FullResult(index, v[i]);
			}
			else if(item is Matrix) {
				Matrix m = item as Matrix;
				int [] i = m.MinIndex();

				Array index = new Array();
				index.Add(i[0]);
				index.Add(i[1]);

				return this.FullResult(index, m[i[0], i[1]]);
			}
			else if(item is Array) {
				Array array = this.EvaluateGoodDepthArray(depth, item as Array, arguments);
				this.CheckResultLength(array, depth);

				int ind = 0;
				double min = (double)(array[0] as object [])[1];

				for(int i = 1; i < array.Count; i++) {
					double d = (double)(array[i] as object [])[1];
					if(d < min) {
						ind = i;
						min = d;
					}
				}

				Array index = (array[ind] as object [])[0] as Array;
				index.Insert(0, ind);

				return this.FullResult(index, min);
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací index prvku s nejnižší èíselnou hodnotou (do zadané hloubky)";
	}
}
