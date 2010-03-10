using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a vector or a matrix with the probability density of the wave function(s)
    /// </summary>
    public class DensityMatrix : Fnc {
        public override string Help { get { return Messages.HelpDensityMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(3, true);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PEVIndexes, Messages.PEVIndexesDescription, null, typeof(int), typeof(TArray));
            this.SetParam(2, true, true, false, Messages.PInterval, Messages.PIntervalDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem qs = arguments[0] as IQuantumSystem;

            int[] n;
            bool one = false;

            if(arguments[1] is int) {
                n = new int[1];
                n[0] = (int)arguments[1];

                one = true;
            }
            else {
                TArray a = arguments[1] as TArray;

                int num = a.Length;
                n = new int[num];

                for(int j = 0; j < num; j++)
                    n[j] = (int)a[j];
            }

            int count = arguments.Count;
            Vector[] interval = new Vector[count - 2];
            for(int i = 2; i < count; i++)
                interval[i - 2] = arguments[i] as Vector;

            object result = qs.ProbabilityDensity(n, guider, interval);

            if(one) {
                if(result is Vector[])
                    return (result as Vector[])[0];
                else if(result is Matrix[])
                    return (result as Matrix[])[0];
            }
            else
                return new TArray(result as Array);

            return null;
        }
    }
}
