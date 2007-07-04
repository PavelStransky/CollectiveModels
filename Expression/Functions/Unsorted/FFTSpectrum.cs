using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Fast Fourier Transform (FFT) of a vector
    /// </summary>
    public class FFTSpectrum: FunctionDefinition {
        public override string Name { get { return Messages.HelpFFTSpectrum; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.PSignal, Messages.PSignalDescription, null, typeof(Vector), typeof(ComplexVector));
            this.SetParam(1, false, true, true, Messages.PSamplingRate, Messages.PSamplingRateDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ComplexVector cv = arguments[0] is Vector ? (arguments[0] as Vector).ToComplexVector() : (arguments[0] as ComplexVector);

            ComplexVector result = FFT.Compute(cv);
            return FFT.PowerSpectrum(result, (double)arguments[1]);
        }
    }
}
