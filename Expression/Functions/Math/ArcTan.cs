using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// ArcTan
	/// </summary>
	public class ArcTan: MathFnD {
		public override string Help {get {return Messages.HelpArcTan;}}

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Atan(x);
        }
	}
}
