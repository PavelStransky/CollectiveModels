using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a vector with Wigner semicircle distributed components
    /// </summary>
    public class RandomVectorC : Fnc {
        public override string Help { get { return Messages.HelpRandomVectorC; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, false, true, true, Messages.PRadius, Messages.PRadiusDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            double r = (double)arguments[1];

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = RMTDistribution.GetSemicircle(r);

            return result;
        }
    }
}
