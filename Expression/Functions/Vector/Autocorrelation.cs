using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the autocorrelation function of a vector up to a given lag t
    /// </summary>
    /// <remarks>
    /// Normalized by the variance of the parts of the vector taken into account
    /// C_t = sum_{i=t}^{N} x_i*x_{i-t} / sqrt[(sum_{i=1}^{N-t}x_i*x_i)*(sum_{i=t}^{N}x_i*x{i)]
    /// </remarks>
    public class Autocorrelation: Fnc {
        public override string Help { get { return Messages.HelpAutocorrelation; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PShift, Messages.PShiftDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];
            int shift = (int)arguments[1];

            int length = v.Length;
            Vector result = new Vector(shift);

            for(int s = 0; s < shift; s++) {
                double sum = 0.0;
                double norm1 = 0.0;
                double norm2 = 0.0;

                for(int i = s; i < length; i++) {
                    double x1 = v[i];
                    double x2 = v[i - s];

                    sum += x1 * x2;
                    norm1 += x1 * x1;
                    norm2 += x2 * x2;
                }

                result[s]=sum/System.Math.Sqrt(norm1 * norm2);
            }

            return result;
        }
    }
}