using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the smooth level density by the Strutinsky method
    /// </summary>
    public class SmoothLevelDensity: FncMathD {
        public override string Help { get { return Messages.HelpSmoothLevelDensity; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PStrutinsky, Messages.PStrutinskyDescription, null, typeof(Strutinsky));
        }

        protected override double FnDouble(double x, params object[] p) {
            Strutinsky s = p[0] as Strutinsky;
            return s.SmoothLevelDensity(x);
        }
    }
}