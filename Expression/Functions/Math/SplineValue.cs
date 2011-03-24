using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the Spline interpolation
    /// </summary>
    public class SplineValue: FncMathD {
        public override string Help { get { return Messages.HelpSplineValue; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PSpline, Messages.PSplineDescription, null, typeof(Spline));
        }

        protected override double FnDouble(double x, params object[] p) {
            Spline spline = p[0] as Spline;
            return spline.GetValue(x);
        }
    }
}
