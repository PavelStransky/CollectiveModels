using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Value of the Legendre polynomial
    /// </summary>
    public class Legendre: MathFnD {
        public override string Help { get { return Messages.HelpLaguerre; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, true, Messages.PX, Messages.PXDetail, null, typeof(double));
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Legendre(x, (int)p[0]);
        }
    }
}
