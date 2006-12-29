using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací index prvku s nejnižší èíselnou hodnotou v absolutní hodnotì
	/// </summary>
	public class MinAbsIndex: FunctionDefinitionMinMaxIndex {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector) {
				Vector v = item as Vector;
				int i = v.MinAbsIndex();

				TArray index = new TArray();
				index.Add(i);

				return this.FullResult(index, v[i]);
			}
			else if(item is Matrix) {
				Matrix m = item as Matrix;
				int [] i = m.MinAbsIndex();

				TArray index = new TArray();
				index.Add(i[0]);
				index.Add(i[1]);

				return this.FullResult(index, m[i[0], i[1]]);
			}
			else if(item is TArray) {
				TArray array = this.EvaluateGoodDepthArray(depth, item as TArray, arguments);
				this.CheckResultLength(array, depth);

				int ind = 0;
				double min = System.Math.Abs((double)(array[0] as object [])[1]);

				for(int i = 1; i < array.Count; i++) {
					double d = System.Math.Abs((double)(array[i] as object [])[1]);
					if(d < min) {
						ind = i;
						min = d;
					}
				}

				TArray index = (array[ind] as object [])[0] as TArray;
				index.Insert(0, ind);

				min = (double)(array[ind] as object [])[0];

				return this.FullResult(index, min);
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací index prvku s nejnižší èíselnou hodnotou v absolutní hodnotì (do zadané hloubky)";
	}
}
