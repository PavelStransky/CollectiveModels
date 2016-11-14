using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns all possible convolutions of functions defined for x > 0 of depth d
    /// </summary>
    public class ConvolutionA : Fnc {
        public override string Help { get { return Messages.HelpConvolutionA; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PArray, Messages.PArrayDescription, null, typeof(TArray));
            this.SetParam(1, true, true, false, Messages.PDepth, Messages.PDepthDescription, 1, typeof(int));
            this.SetParam(2, false, true, true, Messages.PStep, Messages.PStepDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray f = arguments[0] as TArray;
            int depth = (int)arguments[1];
            double d = (double)arguments[2];

            List result = new List();

            if (guider != null)
                guider.Write("[0");
                                                                                                                                          
            for (int i = 0; i < f.Length; i++) {
                this.Recursive(result, f, f[i] as Vector, i, depth - 1, d, string.Format("({0}", i), guider);
            }

            if (guider != null)
                guider.WriteLine("]");

            return result.ToTArray();
        }

        /// <summary>
        /// Krok rekurze
        /// </summary>
        private void Recursive(List result, TArray f, Vector c, int i, int depth, double d, string text, Guider guider) {
            if (depth == 0) {
                result.Add(c);
                if (guider != null) {
                    guider.Write(string.Format("{0})][", text));
                    guider.Write(result.Count);
                }
            }
            else {
                for (int j = i; j < f.Length; j++) {
                    if (guider != null)
                        guider.Write(".");
                    this.Recursive(result, f, this.Convolute(f[j] as Vector, c, d), j, depth - 1, d, string.Format("{0},{1}", text, j), guider);
                }
            }
        }

        /// <summary>
        /// Provede konvoluci
        /// </summary>
        private Vector Convolute(Vector v1, Vector v2, double d) {
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