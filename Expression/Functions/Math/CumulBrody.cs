using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of cumulative Brody distribution
    /// </summary>
    public class CumulBrody: FncMathD {
        public override string Help { get { return Messages.HelpCumulBrody; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, true, Messages.PBrody, Messages.PBrodyDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            return SpecialFunctions.CumulBrody(x, (double)p[0]);
        }
    }
}
