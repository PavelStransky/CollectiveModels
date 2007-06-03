using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Logarithm of the value (with specified base)
    /// </summary>
    public class Log: MathFnD {
        public override string Help { get { return Messages.LogHelp; } }

        protected override void CreateParameters() {
            this.NumParams(2);

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
