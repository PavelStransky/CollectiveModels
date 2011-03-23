using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the number variance
    /// </summary>
    public class NumberVariance: Fnc {
        public override string Help { get { return Messages.HelpNumberVariance; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PInterval, Messages.PIntervalDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector data = (arguments[0] as Vector).Sort() as Vector;
            Vector lengths = arguments[1] as Vector;

            int length = data.Length;
            int lengthL = lengths.Length;

            Vector mean = new Vector(lengthL);
            Vector variance = new Vector(lengthL);

            for(int i = 0; i < lengthL; i++) {
                double L = lengths[i];
                int numInterval = (int)(length / L);
                Vector v = new Vector(numInterval);
                for(int j = 0; j < length; j++) {
                    int index = (int)(data[j] / L);
                    if(index >= 0 && index < numInterval)
                        v[index]++;
                }

                for(int j = 0; j < numInterval; j++) {
                    v[j] -= L;
                    v[j] *= v[j];
                }

                mean[i] = v.Mean();
                variance[i] = v.Variance();
            }

            List result = new List();
            result.Add(new PointVector(lengths, mean));
            result.Add(variance);
            return result;
        }
    }
}
