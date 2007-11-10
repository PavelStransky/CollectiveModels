using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Value of Wigner distribution
	/// </summary>
	public class Wigner: FncMathD {
		public override string Help {get {return Messages.HelpWigner;}}

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Wigner(x);
        }
	}
}
