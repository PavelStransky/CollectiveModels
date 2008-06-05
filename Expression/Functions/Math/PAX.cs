using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the probability amplitude of the wave function in the required point (only 1D system)
    /// </summary>
    public class PAX: FncMathD {
        public override string Help { get { return Messages.HelpPAX; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(2, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(int));
        }

        protected override double FnDouble(double x, params object[] p) {
            IQuantumSystem qs = p[0] as IQuantumSystem;
            int n = (int)p[1];

            return qs.ProbabilityAmplitude(n, null, x);
        }
    }
}
