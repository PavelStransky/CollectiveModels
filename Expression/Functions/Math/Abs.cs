using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Absolute value
	/// </summary>
	public class Abs: FncMathID {
		public override string Help {get {return Messages.HelpAbs;}}

        protected override int FnInt(int i, params object[] p) {
            return System.Math.Abs(i);
        }

        protected override double FnDouble(double x, params object[] p) {
            return System.Math.Abs(x);
        }
	}
}
