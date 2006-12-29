using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací prvek s nejnižší èíselnou hodnotou
	/// </summary>
	public class Min: FunctionDefinitionMinMax {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector)
				return (item as Vector).Min();
			else if(item is Matrix) 
				return (item as Matrix).Max();
			else if(item is TArray) {
				TArray result = this.EvaluateArray(depth, item as TArray, arguments);
				this.CheckResultLength(result, depth);

				double min = (double)result[0];
				for(int i = 1; i < result.Count; i++) {
					double d = (double)result[i];
					if(d < min)
						min = d;
				}

				return min;
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací prvek s nejnižší èíselnou hodnotou (do zadané hloubky)";
	}
}
