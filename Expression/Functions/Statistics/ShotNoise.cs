using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a vector with shot noise values
    /// </summary>
    /// <remarks>Phys. Rev. E 82, 021109 (2010), formula (3)</remarks>
    public class FnShotNoise: Fnc {
        private ShotNoise sn = new ShotNoise();

        public override string Help { get { return Messages.HelpShotNoise; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, true, true, true, Messages.PAmplitudes, Messages.PAmplitudes, null, typeof(Vector));
            this.SetParam(2, false, true, true, Messages.PRelaxationRate, Messages.PUpperBoundDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PPoissonianIntensity, Messages.PPoissonianIntensityDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            Vector amplitudes = (Vector)arguments[1];

            double r = (double)arguments[2];
            double lambda = (double)arguments[3];

            return this.sn.GetVector(length, amplitudes, r, lambda);
        }

        private static string name = "shotnoise";
    }
}
