using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Hilbert Transform of a time series
    /// </summary>
    public class HilbertTransform: Fnc {
        public override string Help { get { return Messages.HelpHilbertTransform; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PSignal, Messages.PSignalDescription, null, typeof(Vector));
            this.SetParam(1, false, true, true, Messages.PSamplingRate, Messages.PSamplingRateDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ComplexVector x = (arguments[0] as Vector).ToComplexVector();
            int length = x.Length;

            ComplexVector ht = FFT.Compute(x);

            for(int i = 0; i < length; i++) {
                if(i == 0)
                    ht[i] = ht[i];
                else if(i <= length / 2)
                    ht[i] *= new Complex(0, -1);
                else
                    ht[i] *= new Complex(0, 1);
            }

            ht = InvFFT.Compute(ht);

            Vector y = ht.VectorRe;
            y.Length = length;
            return y;
        }            
    }
}
