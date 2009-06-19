using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.CHO;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a Coupled harmonic oscillator class
    /// </summary>
    public class CHO : Fnc {
        public override string Help { get { return Messages.HelpCHO; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, true, Messages.PCouplingConstant, Messages.PCouplingConstantDescription, null, typeof(double));
            this.SetParam(1, false, true, true, Messages.PMass, Messages.PMassDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PRigidity, Messages.PRigidityDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double lambda = (double)arguments[0];
            double m = (double)arguments[1];
            double k = (double)arguments[2];

            return new CoupledHarmonicOscillator(m, k, lambda);
        }
    }
}