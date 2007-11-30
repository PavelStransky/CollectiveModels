using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Replaces a specified value with other one
    /// </summary>
    public class ReplaceValue: FncMathD {
        public override string Help { get { return Messages.HelpRepaceValue; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetXParam();
            this.SetParam(1, true, true, true, Messages.POldValue, Messages.POldValueDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PNewValue, Messages.PNewValueDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            double oldValue = (double)p[0];
            double newValue = (double)p[1];

            return x == oldValue ? newValue : x;
        }
    }
}
