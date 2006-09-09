using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� prvek s nejni��� ��selnou hodnotou v absolutn� hodnot�
	/// </summary>
	public class MinAbs: FunctionDefinitionMinMax {
		public override string Help {get {return help;}}

		protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
			if(item is int)
				return (double)(int)item;
			else if(item is double)
				return item;
			else if(item is Vector)
				return (item as Vector).MinAbs();
			else if(item is Matrix) 
				return (item as Matrix).MinAbs();
			else if(item is Array) {
				Array result = this.EvaluateArray(depth, item as Array, arguments);
				this.CheckResultLength(result, depth);
				
				int index = 0;
				double min = System.Math.Abs((double)result[0]);

				for(int i = 1; i < result.Count; i++) {
					double d = System.Math.Abs((double)result[i]);
					if(d < min) {
						index = i;
						min = d;
					}
				}

				return result[index];
			}
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrac� prvek s nejni��� ��selnou hodnotou v absolutn� hodnot� (do zadan� hloubky)";
	}
}
