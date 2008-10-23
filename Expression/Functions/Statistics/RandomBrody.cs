using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value with Brody distribution
    /// </summary>
    public class RandomBrody: Fnc {
        public override string Help { get { return Messages.HelpRandomBrody; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, true, Messages.PBrody, Messages.PBrodyDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return RMTDistribution.GetBrody((double)arguments[0]);
        }
    }
}
