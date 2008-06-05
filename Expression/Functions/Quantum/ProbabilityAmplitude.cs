using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the probability density of the wave function in the required point
    /// </summary>
    public class ProbabilityAmplitude: Fnc {
        public override string Help { get { return Messages.HelpProbabilityAmplitude; } }

        protected override void CreateParameters() {
            this.SetNumParams(3, true);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(int));
            this.SetParam(2, true, true, false, Messages.PValue, Messages.PValueDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem qs = arguments[0] as IQuantumSystem;
            int n = (int)arguments[1];

            int count = arguments.Count;
            double[] x = new double[count - 2];
            for(int i = 2; i < count; i++)
                x[i - 2] = (double)arguments[i];

            return qs.ProbabilityAmplitude(n, guider, x);
        }
    }
}
