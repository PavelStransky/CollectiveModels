using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a given number of zeros of the spherical Bessel function j
    /// </summary>
    public class SphericalBesselJZero: Fnc {
        public override string Help { get { return Messages.HelpSphericalBesselJZero; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, true, Messages.POrder, Messages.POrderDetail, null, typeof(double));
            this.SetParam(1, true, true, false, Messages.PZeroNumber, Messages.PZeroNumberDescription, null, typeof(int));
            this.SetParam(2, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-6, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double order = (double)arguments[0];
            int num = (int)arguments[1];
            double precision = (double)arguments[2];

            BesselJZero b = new BesselJZero(order, precision);
            return b.Solve(num);
        }
    }
}
