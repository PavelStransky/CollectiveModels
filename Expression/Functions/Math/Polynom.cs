using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Data of input vector interprets as coeficients of polynom and return its value
	/// </summary>
	public class FnPolynom: MathFnD {
		public override string Name {get {return name;}}
		public override string Help {get {return Messages.PolynomHelp;}}

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PPolynomCoef, Messages.PPolynomCoefDescription, null, typeof(Vector));
        }

        protected override double FnDouble(double x, params object[] p) {
            return Polynom.GetValue(p[0] as Vector, x);
        }

		private const string name = "polynom";
	}
}