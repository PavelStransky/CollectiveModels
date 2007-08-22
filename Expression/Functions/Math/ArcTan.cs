using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// ArcTan
	/// </summary>
	public class ArcTan: FncMathD {
		public override string Help {get {return Messages.HelpArcTan;}}

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Atan(x);
        }
	}
}
