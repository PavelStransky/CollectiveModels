using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Value of Wigner GOE distribution
	/// </summary>
	public class GOE: FncMathD {
		public override string Help {get {return Messages.HelpGOE;}}

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.GOE(x);
        }
	}
}
