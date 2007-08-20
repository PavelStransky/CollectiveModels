using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Value of the Laguerre polynomial
    /// </summary>
    public class Laguerre : MathFnD {
        public override string Help { get { return Messages.HelpLaguerre; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PAssociatedOrder, Messages.PAssociatedOrderDetail, 0, typeof(int));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Laguerre(x, (int)p[0], (int)p[1]);
        }
    }
}
