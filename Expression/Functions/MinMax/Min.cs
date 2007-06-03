using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� prvek s nejni��� ��selnou hodnotou
	/// </summary>
	public class Min: MinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).Min();
            else
                return (item as Matrix).Min();
        }

		private const string help = "Vrac� prvek s nejni��� ��selnou hodnotou";
	}
}
