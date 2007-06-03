using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací prvek s nejvyšší èíselnou hodnotou
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

		private const string help = "Vrací prvek s nejvyšší èíselnou hodnotou";
	}
}
