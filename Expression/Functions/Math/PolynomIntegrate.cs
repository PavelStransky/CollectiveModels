using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Data of input vector interprets as coeficients of polynom and return its value
    /// </summary>
    public class PolynomIntegrate: FncMathD {
        public override string Help { get { return Messages.HelpPolynomIntegrate; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PPolynomCoef, Messages.PPolynomCoefDescription, null, typeof(Vector));
        }

        protected override double FnDouble(double x, params object[] p) {
            return Polynom.GetIValue(p[0] as Vector, x);
        }
    }
}