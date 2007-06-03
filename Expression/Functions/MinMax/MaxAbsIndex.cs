using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� index prvku s nejvy��� ��selnou hodnotou v absolutn� hodnot�
	/// </summary>
	public class MaxAbsIndex: MinMax {
		public override string Help {get {return help;}}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).MaxAbsIndex();
            else
                return new TArray((item as Matrix).MaxAbsIndex());
        }

		private const string help = "Vrac� index prvku s nejvy��� ��selnou hodnotou v absolutn� hodnot�";
	}
}