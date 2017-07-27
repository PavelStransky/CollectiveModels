using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a symmetric matrix with Gaussian distributed components
    /// (according to PRL 65, 529 (1990))
    /// </summary>
    public class RandomMatrixSG: Fnc {
        private NormalDistribution nd = new NormalDistribution();

        public override string Help { get { return Messages.HelpRandomMatrixSG; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PMatrixSize, Messages.PMatrixSizeDescription, null, typeof(int));
            this.SetParam(1, false, true, true, Messages.PVariance, Messages.PVarianceDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            int ndLength = length * (length - 1) / 2;

            double var = (double)arguments[1];

            Vector diag = this.nd.GetVector(length, var * System.Math.Sqrt(2.0), 0.0);
            Vector ndiag = this.nd.GetVector(ndLength, var, 0.0);

            Matrix result = new Matrix(length);
            int k = 0;
            for(int i = 0; i < length; i++) {
                result[i, i] = diag[i];

                for(int j = i + 1; j < length; j++)
                    result[i, j] = result[j, i] = ndiag[k++];
            }

            return result;
        }
    }
}
