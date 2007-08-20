using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Generates an envelope matrix in Gaussian form
    /// (according to PRL 65, 529 (1990))
    /// </summary>
    public class EnvelopeMatrixG: FunctionDefinition {
        public override string Help { get { return Messages.HelpEnvelopeMatrixG; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PSize, Messages.PSizeDescription, null, typeof(int));
            this.SetParam(1, true, true, true, Messages.PVariance, Messages.PVarianceDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            double variance = (double)arguments[1];

            if(variance == 0.0)
                return Matrix.Unit(length);

            double denominator1 = 1.0 / System.Math.Sqrt(2.0 * System.Math.PI * variance * variance);
            double denominator2 = -1.0 / (2.0 * variance * variance);

            Matrix result = new Matrix(length);
            for(int i = 0; i < length; i++)
                for(int j = i; j < length; j++) {
                    int k = i - j;
                    double d = denominator1 * System.Math.Exp(denominator2 * k * k);
                    result[i, j] = d;
                    result[j, i] = d;
                }

            return result;
        }
    }
}
