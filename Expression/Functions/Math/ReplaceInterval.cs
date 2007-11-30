using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Replaces values from specified interval with new value
    /// </summary>
    public class ReplaceInterval: FncMathD {
        public override string Help { get { return Messages.HelpReplaceInterval; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetXParam();
            this.SetParam(1, true, true, true, Messages.POldValueMin, Messages.POldValueMinDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.POldValueMax, Messages.POldValueMaxDescription, null, typeof(double));
            this.SetParam(3, true, true, true, Messages.PNewValue, Messages.PNewValueDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            double oldValueMin = (double)p[0];
            double oldValueMax = (double)p[1];
            double newValue = (double)p[2];

            return (x >= oldValueMin && x <= oldValueMax) ? newValue : x;
        }
    }
}
