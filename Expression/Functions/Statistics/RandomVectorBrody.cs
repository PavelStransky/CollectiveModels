using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a vector with Brody distributed components
    /// </summary>
    public class RandomVectorBrody: Fnc {
        public override string Help { get { return Messages.HelpRandomVectorBrody; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, true, true, true, Messages.PBrody, Messages.PBrodyDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            double brody = (double)arguments[1];

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = RMTDistribution.GetBrody(brody);

            return result;
        }
    }
}
