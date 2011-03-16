using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the Laguerre polynomial
    /// </summary>
    public class Laguerre : FncMathD {
        public override string Help { get { return Messages.HelpLaguerre; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
            this.SetParam(2, false, true, true, Messages.PAssociatedOrder, Messages.PAssociatedOrderDetail, 0, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Laguerre(x, (int)p[0], (double)p[1]);
        }
    }
}
