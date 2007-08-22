using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Logarithm of the value (with specified base)
    /// </summary>
    public class Log: FncMathD {
        public override string Help { get { return Messages.HelpLog; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, false, true, true, Messages.PBase, Messages.PBaseDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            if(x == 0.0)
                return 0.0;

            if(p[0] != null)
                return System.Math.Log(x, (double)p[0]);
            else
                return System.Math.Log(x);
        }
    }
}
