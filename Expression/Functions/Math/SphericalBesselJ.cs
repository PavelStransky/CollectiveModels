using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the spherical BesselJ function
    /// </summary>
    public class SphericalBesselJ: FncMathD {
        public override string Help { get { return Messages.HelpSphericalBesselJ; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, true, Messages.POrder, Messages.POrderDetail, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.SphericalBesselFunction(x, (double)p[0])[0];
        }
    }
}
