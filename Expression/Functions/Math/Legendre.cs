using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the Legendre polynomial
    /// </summary>
    public class Legendre: FncMathD {
        public override string Help { get { return Messages.HelpLaguerre; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.Legendre(x, (int)p[0]);
        }
    }
}
