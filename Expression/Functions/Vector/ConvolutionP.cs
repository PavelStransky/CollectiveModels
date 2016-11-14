using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a convolution of functions defined for x > 0
    /// </summary>
    public class ConvolutionP : Fnc {
        public override string Help { get { return Messages.HelpConvolutionP; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PArray, Messages.PArrayDescription, null, typeof(TArray));
            this.SetParam(1, false, true, true, Messages.PStep, Messages.PStepDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray f = arguments[0] as TArray;
            double d = (double)arguments[1];

            TArray result = new TArray(typeof(Vector), f.Length - 1);
            Vector v = f[0] as Vector;
            for (int i = 1; i < f.Length; i++) {
                v = this.Convolute(d, v, f[i] as Vector);
                if (guider != null)
                    guider.Write(".");
                result[i - 1] = v;
            }

            return result;
        }

        /// <summary>
        /// Provede integraci pod křivkou (lichoběžníkové pravidlo)
        /// </summary>
        private Vector Convolute(double d, Vector v1, Vector v2) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for (int i = 1; i < length; i++) {
                int imax = i;
                for (int j = 1; j <= imax; j++)
                    result[i] += (v1[j - 1] * v2[imax - j + 1] + v1[j] * v2[imax - j]) / 2.0;
            }
            return d * result;
        }
    }
}