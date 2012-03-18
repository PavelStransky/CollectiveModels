using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// ArcTan
	/// </summary>
	public class ArcTan: FncMathD {
		public override string Help {get {return Messages.HelpArcTan;}}

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetXParam();
            this.SetParam(1, false, true, true, Messages.PY, Messages.PYDescription, null, typeof(double));
        }
        
        protected override double FnDouble(double x, params object[] p) {
            if(p.Length > 0)
                return System.Math.Atan2(x, (double)p[0]);
            else
                return System.Math.Atan(x);
        }
	}
}
