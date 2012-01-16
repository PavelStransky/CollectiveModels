using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Instantaneous frequency between two vectors
    /// </summary>
    public class InstantaneousFrequency: Fnc {
        public override string Help { get { return Messages.HelpInstantaneousFrequency; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PReal, Messages.PRealDescription, null, typeof(Vector));
            this.SetParam(2, true, true, false, Messages.PImmaginary, Messages.PImmaginaryDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector x = (Vector)arguments[0];

            Vector v1 = (Vector)arguments[1];
            Vector v2 = (Vector)arguments[2];

            int length = x.Length;

            if(length != v1.Length && length != v2.Length)
                throw new FncException(
                    this,
                    Messages.EMNotEqualLength,
                    string.Format(Messages.EMNotEqualLengthDetail3, length, v1.Length, v2.Length));

            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++) {
                PointD dx = this.Derivative(x, v1, i);
                PointD dy = this.Derivative(x, v2, i);

                result[i] = new PointD(dx.X, (dx.Y * v2[i] - dy.Y * v1[i]) / (v1[i] * v1[i] + v2[i] * v2[i]));
            }

            return result;
        }

        private PointD Derivative(Vector x, Vector y, int i) {
            if(i == 0)
                return new PointD(x[0], (y[1] - y[0]) / (x[1] - x[0]));

            int last = x.Length - 1;
            if(i == last)
                return new PointD(x[last], (y[last] - y[last - 1]) / (x[last] - x[last - 1]));

            return new PointD((x[i - 1] + x[i + 1]) / 2.0, (y[i + 1] - y[i - 1]) / (x[i + 1] - x[i - 1]));
        }
    }
}
