using System;
using System.Collections;

using PavelStransky.DLLWrapper;
using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a random matrix for the superradiance study (random rotation with d open channels)
    /// </summary>
    public class RandomMatrixSR : Fnc {
        private NormalDistribution nd = new NormalDistribution();
        private Random r = new Random();

        public override string Help { get { return Messages.HelpRandomMatrixSG; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PMatrixSize, Messages.PMatrixSizeDescription, null, typeof(int));
            this.SetParam(1, true, true, false, Messages.POpenChannels, Messages.POpenChannelsDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            // Generování náhodné matice
            int length = (int)arguments[0];
            int ndLength = length * (length - 1) / 2;

            Vector diag = this.nd.GetVector(length, System.Math.Sqrt(2.0), 0.0);
            Vector ndiag = this.nd.GetVector(ndLength, 1, 0);

            MMatrix goe = new MMatrix(length);
            int k = 0;
            for (int i = 0; i < length; i++) {
                goe[i, i] = diag[i];

                for (int j = i + 1; j < length; j++)
                    goe[i, j] = goe[j, i] = ndiag[k++];
            }

            Vector[] eigenSystem = LAPackDLL.dsyev(goe, true);

            Matrix o = new Matrix(length);
            for (int i = 0; i < length; i++)
                o.SetColumnVector(i, eigenSystem[i + 1]);

            // Matice d
            int open = (int)arguments[1];
            Matrix diagonal = new Matrix(length);

            while (open > 0) {
                int i = r.Next(length);
                if (diagonal[i, i] == 0) {
                    diagonal[i, i]++;
                    open--;
                }
            }

            return o * diagonal * o.Transpose();
        }
    }
}
