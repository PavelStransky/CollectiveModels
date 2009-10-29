using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Square root
    /// </summary>
    public class Sqrt: FncMathD {
        public override string Help { get { return Messages.HelpSqrt; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetXParam();
            this.SetParam(1, false, true, false, Messages.POrderOfSquareRoot, Messages.POrderOfSquareRootDescription, 1, typeof(double), typeof(int));
        }

        protected override double FnDouble(double x, params object[] p) {
            object order = p[0];
            if (order is int) {
                int oi = (int)order;
                if (oi == 1)
                    return System.Math.Sqrt(x);
                else if (oi % 2 == 1)
                    return System.Math.Sign(x) * System.Math.Pow(System.Math.Abs(x), 1.0 / oi);
                else
                    return System.Math.Pow(x, 1.0 / oi);
            }
            else if(order is double) {
                double od = (double)order;
                if(od == 1.0)
                    return System.Math.Sqrt(x);
                else
                    return System.Math.Pow(x, 1.0 / od);
            }

            return double.NaN;
        }
    }
}
