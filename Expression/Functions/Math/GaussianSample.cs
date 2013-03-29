using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the convolution of the input vector with the Gaussian function of a given standard deviation and sample it at the values of a given vector
    /// </summary>
    public class GaussianSample : Fnc {
        public override string Help { get { return Messages.HelpGaussianSample; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector), typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PSamplingPoints, Messages.PSamplingPointsDescription, null, typeof(Vector));
            this.SetParam(2, false, true, true, Messages.PVariance, Messages.PVarianceDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector input = null;
            Vector val = null;
            if(arguments[0] is Vector) {
                input = (Vector)arguments[0];
                val = new Vector(input.Length);
                for(int i = 0; i < input.Length; i++)
                    val[i] = 1.0;
            }
            else {
                input = (arguments[0] as PointVector).VectorX;
                val = (arguments[0] as PointVector).VectorY;
            }

            Vector x = (Vector)arguments[1];
            double var = (double)arguments[2];
 
            int length = x.Length;
            PointVector result = new PointVector(length);

            double x1 = x[0] - 0.5 * (x[1] - x[0]);
            double sqrt2s = var * System.Math.Sqrt(2.0);

            for(int i = 0; i < length; i++) {
                double x2 = i + 1 < length ? 0.5 * (x[i + 1] + x[i]) : x[i] + 0.5 * (x[i] - x[i - 1]);

                double y = 0;
                for(int j = 0; j < input.Length; j++)
                    y += val[j] * 0.5 * (SpecialFunctions.Erf1((x2 - input[j]) / sqrt2s) - SpecialFunctions.Erf1((x1 - input[j]) / sqrt2s));

                result[i] = new PointD(x[i], y);

                x1 = x2;
            }

            return result;
        }
    }
}