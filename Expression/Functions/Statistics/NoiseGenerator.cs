using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a vector with a 1/f^beta noise
    /// </summary>
    /// <remarks>Ruben Fossion's generator</remarks>
    public class NoiseGenerator: Fnc {
        public override string Help { get { return Messages.HelpNoiseGenerator; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, true, true, true, Messages.PBeta, Messages.PBetaDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, false, Messages.PPhase, Messages.PPhaseDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            double beta = (double)arguments[1];
            Vector phases = (Vector)arguments[2];

            double coef = 2.0 * System.Math.PI / length;
            double alpha = -beta / 2.0;

            if(phases == null)
                phases = new Vector(0);
            phases.Length = length / 2 + 1;

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++) {
                double d = 0.0;
                for(int k = 1; k <= length / 2; k++)
                    d += System.Math.Pow(k, alpha) * System.Math.Sin(coef * k * i + phases[k]);
                result[i] = d;
            }

            return result;
        }
    }
}
