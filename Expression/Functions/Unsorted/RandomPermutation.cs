using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates an array of random permutated numbers
    /// </summary>
    public class RandomPermutation : Fnc {
        public override string Help { get { return Messages.HelpRandomPermutation; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];

            double[] d = new double[length];
            int[] i = new int[length];

            Random r = new Random();

            for(int j = 0; j < length; j++) {
                d[j] = r.NextDouble();
                i[j] = j;
            }

            Array.Sort(d, i);
            return new TArray(i);
        }
    }
}