using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací index prvku s nejnižší èíselnou hodnotou
	/// </summary>
	public class MinIndex: MinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).MinIndex();
            else
                return (item as Matrix).MinIndex();
        }

		private const string help = "Vrací index prvku s nejnižší èíselnou hodnotou";
	}
}
