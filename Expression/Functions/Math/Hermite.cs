using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the Hermite polynomial
    /// </summary>
    public class Hermite: FncMathD {
        public override string Help { get { return Messages.HelpHermite; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, true, Messages.PX, Messages.PXDetail, null, typeof(double));
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Hermite(x, (int)p[0]);
        }
    }
}
