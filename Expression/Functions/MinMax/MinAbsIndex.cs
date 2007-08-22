using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Vrací index prvku s nejnižší èíselnou hodnotou v absolutní hodnotì
	/// </summary>
	public class MinAbsIndex: FncMinMax {
		public override string Help {get {return help;}}


        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).MinAbsIndex();
            else
                return (item as Matrix).MinAbsIndex();
        }

		private const string help = "Vrací index prvku s nejnižší èíselnou hodnotou v absolutní hodnotì";
	}
}
