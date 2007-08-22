using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Vrací prvek s nejnižší èíselnou hodnotou v absolutní hodnotì
	/// </summary>
	public class MinAbs: FncMinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).MinAbs();
            else
                return (item as Matrix).MinAbs();
        }
        
        private const string help = "Vrací prvek s nejnižší èíselnou hodnotou v absolutní hodnotì";
	}
}
