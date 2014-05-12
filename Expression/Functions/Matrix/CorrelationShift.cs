using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the position of the maximum correlation
    /// </summary>
    public class CorrelationShift : Fnc {
        public override string Help { get { return Messages.HelpCorrelationShift; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.PSizeWindow, Messages.PSizeWindowDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int window = (int)arguments[1];

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

            if(window == 0)
                window = lengthY / 3;

            int lengthYh = lengthY - window;

            Matrix maxIndex = new Matrix(lengthX);
            Matrix maxValue = new Matrix(lengthX);

            for(int i = 0; i < lengthX; i++) {
                Vector v1 = this.GetNormVector(m, i, lengthYh / 2, window);
                
                for(int j = 0; j < lengthX; j++) {
                    Vector corr = new Vector(lengthYh + 1);

                    for(int k = 0; k <= lengthYh; k++) {
                        Vector v2 = this.GetNormVector(m, j, k, window);
                        corr[k] = v1 * v2;
                    }

                    int l = corr.MaxAbsIndex();
                    maxIndex[i, j] = l - lengthYh / 2;
                    maxValue[i, j] = corr[l] / window;
                }
            }

            List result = new List();
            result.Add(maxIndex);
            result.Add(maxValue);
            return result;
        }

        private Vector GetNormVector(Matrix m, int i, int j, int length) {
            Vector v = new Vector(length);

            double mean = 0.0;
            for(int k = 0; k < length; k++) {
                double d = m[i, j + k];
                v[k] = d;
                mean += d;
            }
            mean /= length;

            double var = 0.0;
            for(int k = 0; k < length; k++) {
                double d = v[k] - mean;
                var += d * d;
            }
            var = System.Math.Sqrt(var / length);

            for(int k = 0; k < length; k++)
                v[k] = (v[k] - mean) / var;

            return v;
        }
    }
}
