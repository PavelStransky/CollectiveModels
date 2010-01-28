using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the potential of the system PT1
    /// </summary>
    public class PT1Potential: FncMathD {
        public override string Help { get { return Messages.HelpPT1Potential; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, true, Messages.PMixingParameter, Messages.PMixingParameterDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            return PT1.V(x, (double)p[0]);
        }
    }
}
