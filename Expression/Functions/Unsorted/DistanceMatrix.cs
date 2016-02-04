using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a matrix filled with distances of a given set of points
    /// </summary>
    public class DistanceMatrix : Fnc {
        public override string Help { get { return Messages.HelpDistanceMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int num = arguments.Count;
            Vector x = arguments[0] as Vector;
            int length = x.Length;

            Matrix result = new Matrix(length);

            for(int i = 0; i < length; i++)
                for(int j = i + 1; j < length; j++) {
                    double a = 0.0;
                    for(int k = 0; k < num; k++) {
                        double b = (arguments[k] as Vector)[i] - (arguments[k] as Vector)[j];
                        a += b * b;
                    }
//                    a = System.Math.Sqrt(a);

                    result[i, j] = a;
                    result[j, i] = a;
                }

            return result;
        }
    }
}
