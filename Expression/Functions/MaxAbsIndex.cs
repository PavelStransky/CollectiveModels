using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� index prvku s nejvy��� ��selnou hodnotou v absolutn� hodnot�
	/// </summary>
	public class MaxAbsIndex: FunctionDefinitionMinMaxIndex {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector) {
				Vector v = item as Vector;
				int i = v.MaxAbsIndex();

				TArray index = new TArray();
				index.Add(i);

				return this.FullResult(index, v[i]);
			}
			else if(item is Matrix) {
				Matrix m = item as Matrix;
				int [] i = m.MaxAbsIndex();

				TArray index = new TArray();
				index.Add(i[0]);
				index.Add(i[1]);

				return this.FullResult(index, m[i[0], i[1]]);
			}
			else if(item is TArray) {
				TArray array = this.EvaluateGoodDepthArray(depth, item as TArray, arguments);
				this.CheckResultLength(array, depth);

				int ind = 0;
				double max = System.Math.Abs((double)(array[0] as object [])[1]);

				for(int i = 1; i < array.Count; i++) {
					double d = System.Math.Abs((double)(array[i] as object [])[1]);
					if(d > max) {
						ind = i;
						max = d;
					}
				}

				TArray index = (array[ind] as object [])[0] as TArray;
				index.Insert(0, ind);

				max = (double)(array[ind] as object [])[1];

				return this.FullResult(index, max);
			}
			else
				return this.BadTypeError(item, 0);
		}

		private const string help = "Vrac� index prvku s nejvy��� ��selnou hodnotou v absolutn� hodnot� (do zadan� hloubky)";
	}
}