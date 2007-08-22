using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Vrac� index prvku s nejni��� ��selnou hodnotou v absolutn� hodnot�
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

		private const string help = "Vrac� index prvku s nejni��� ��selnou hodnotou v absolutn� hodnot�";
	}
}
