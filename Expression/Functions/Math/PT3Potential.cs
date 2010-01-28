using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the potential of the system PT3
    /// </summary>
    public class PT3Potential: FncMathD {
        public override string Help { get { return Messages.HelpPT3Potential; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetXParam();
            this.SetParam(1, true, true, true, Messages.PA, Messages.PADescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PB, Messages.PBDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            return PT3.V(x, (double)p[0], (double)p[1]);
        }
    }
}
