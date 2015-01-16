using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a vector containing all the possible sums of elements of given vectors
    /// </summary>
    public class CombineVector : Fnc {
        public override string Help { get { return Messages.HelpCombineVector; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PArray, Messages.PArrayDescription, null, typeof(TArray));
            this.SetParam(1, false, true, true, Messages.PMin, Messages.PMinDescription, double.MinValue, typeof(double));
            this.SetParam(2, false, true, true, Messages.PMax, Messages.PMaxDescription, double.MaxValue, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray vs = arguments[0] as TArray;
            double min = (double)arguments[1];
            double max = (double)arguments[2];

            int length = this.RecursionCount(vs, vs.Length, min, max, 0.0, guider);

            if(guider != null)
                guider.WriteLine(length);

            Vector result = new Vector(length);
            int k = 0;
            this.RecursionFill(result, ref k, vs, vs.Length, min, max, 0.0, guider);
            return result;
        }

        private int RecursionCount(TArray vs, int i, double min, double max, double sum, Guider guider) {
            int result = 0;
            
            i--;

            Vector v = vs[i] as Vector;
            int length = v.Length;
            int coef = length / 100;

            for(int j = 0; j < length; j++) {
                if(i > 0)
                    result += this.RecursionCount(vs, i, min, max, sum + v[j], null);
                else if(sum > min && sum <= max)
                    result++;

                if(guider != null && j % coef == 0)
                    guider.Write('.');
            }

            return result;
        }

        private void RecursionFill(Vector result, ref int k, TArray vs, int i, double min, double max, double sum, Guider guider) {
            if(i > 0) {
                i--;

                Vector v = vs[i] as Vector;
                int length = v.Length;
                int coef = length / 100;

                for(int j = 0; j < length; j++) {
                    this.RecursionFill(result, ref k, vs, i, min, max, sum + v[j], null);
                    if(guider != null && j % coef == 0)
                        guider.Write('.');
                }
            }
            else {
                if(sum > min && sum <= max)
                    result[k++] = sum;
            }                
        }
    }
}