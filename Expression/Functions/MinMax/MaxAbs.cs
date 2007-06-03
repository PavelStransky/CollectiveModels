using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací prvek s nejvyšší èíselnou hodnotou v absolutní hodnotì
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
        
        private const string help = "Vrací prvek s nejvyšší èíselnou hodnotou v absolutní hodnotì";
	}
}
