using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� prvek s nejvy��� ��selnou hodnotou v absolutn� hodnot�
	/// </summary>
	public class MaxAbs: MinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).MaxAbs();
            else
                return (item as Matrix).MaxAbs();
        }
        
        private const string help = "Vrac� prvek s nejvy��� ��selnou hodnotou v absolutn� hodnot�";
	}
}
