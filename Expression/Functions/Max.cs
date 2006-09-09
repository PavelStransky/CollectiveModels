using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací prvek s nejvyšší èíselnou hodnotou
	/// </summary>
	public class Max: FunctionDefinitionMinMax {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector)
				return (item as Vector).Max();
			else if(item is Matrix) 
				return (item as Matrix).Max();
			else if(item is Array) {
				Array result = this.EvaluateArray(depth, item as Array, arguments);
				this.CheckResultLength(result, depth);
				
				double max = (double)result[0];
				for(int i = 1; i < result.Count; i++) {
					double d = (double)result[i];
					if(d > max)
						max = d;
				}

				return max;
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací prvek s nejvyšší èíselnou hodnotou (do zadané hloubky)";
	}
}
