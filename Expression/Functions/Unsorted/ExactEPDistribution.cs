using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the exact EP distribution according to PRE 2017
    /// </summary>
    public class ExactEPDistribution : FncMathD {
        public override string Help { get { return Messages.HelpExactEPDistribution; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PEnergies, Messages.PEnergiesDescription, null, typeof(Vector));
            this.SetParam(2, false, true, true, Messages.PVariance, Messages.PVarianceDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, false, Messages.PType, Messages.PTypeDescription, true, typeof(bool));
        }

        protected override double FnDouble(double x, params object[] p) {
            Vector es = p[0] as Vector;
            double v0 = (double)p[1];
            bool type = (bool)p[2];

            int d = es.Length;
            if(v0 <= 0)
                v0 = System.Math.Sqrt(3.0) * es.Variance();

            double result = 0.0;
            for(int i = 0; i < d; i++)
                for(int j = i + 1; j < d; j++) {
                    double delta = es[j] - es[i];
                    double a = 2.0 * v0 * x / delta;
                    result += (type ? this.Rectangular(a) : this.Normal(a)) / delta;
                }
            return 8.0 * result * v0 / (d * d);
        }

        private double Rectangular(double l) {
            if(l > 1.0)
                return (l - 1.0) / (l * l * l);
            else
                return 0.0;
        }

        private double Normal(double l) {
            if(l <= 0)
                return 0.0;
            double l2i = 1.0 / (l * l);
            return c * System.Math.Exp(-3.0 * l2i) * l2i;
        }

        private static double c = System.Math.Sqrt(3.0 / System.Math.PI);
    }
}
