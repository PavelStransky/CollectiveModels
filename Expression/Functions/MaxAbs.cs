using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací prvek s nejvyšší èíselnou hodnotou v absolutní hodnotì
	/// </summary>
	public class MaxAbs: FunctionDefinitionMinMax {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector)
				return (item as Vector).MaxAbs();
			else if(item is Matrix) 
				return (item as Matrix).MaxAbs();
			else if(item is TArray) {
				TArray result = this.EvaluateArray(depth, item as TArray, arguments);
				this.CheckResultLength(result, depth);
				
				int index = 0;
				double max = System.Math.Abs((double)result[0]);

				for(int i = 1; i < result.Count; i++) {
					double d = System.Math.Abs((double)result[i]);
					if(d > max) {
						index = i;
						max = d;
					}
				}

				return result[index];
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací prvek s nejvyšší èíselnou hodnotou v absolutní hodnotì (do zadané hloubky)";
	}
}
