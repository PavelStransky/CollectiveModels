using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrací index prvku s nejvyšší èíselnou hodnotou
	/// </summary>
	public class MaxIndex: MinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).MaxIndex();
            else
                return new TArray((item as Matrix).MaxIndex());
        }

		private const string help = "Vrací index prvku s nejvyšší èíselnou hodnotou";
	}
}
