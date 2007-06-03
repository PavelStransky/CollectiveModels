using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� prvek s nejvy��� ��selnou hodnotou
	/// </summary>
	public class Max: MinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).Max();
            else
                return (item as Matrix).Max();
		}

		private const string help = "Vrac� prvek s nejvy��� ��selnou hodnotou";
	}
}
