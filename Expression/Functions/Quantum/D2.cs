using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the second derivative
    /// </summary>
    public class D2: Fnc {
        public override string Help { get { return Messages.HelpD2; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, false, true, true, Messages.PStartingPoint, Messages.PStartingPointDetail, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PStep, Messages.PStepDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = arguments[0] as Vector;
            double x0 = (double)arguments[1];
            double step = (double)arguments[2];

            x0 += step;

            int length = v.Length - 2;
            double step2 = step * step;

            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++) {
                result[i].X = x0 + i * step;
                result[i].Y = (v[i] - 2.0 * v[i + 1] + v[i + 2]) / step2;
            }

            return result;
        }
    }
}