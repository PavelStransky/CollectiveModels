using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates a vector with Wigner GSE distributed components
    /// </summary>
    public class RandomVectorGSE: Fnc {
        public override string Help { get { return Messages.HelpRandomVectorGSE; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = RMTDistribution.GetGSE();

            return result;
        }
    }
}
