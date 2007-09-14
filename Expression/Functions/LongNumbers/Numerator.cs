using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Numerator of a fraction
    /// </summary>
    public class Denominator: Fnc {
        public override string Help { get { return Messages.HelpNumerator; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PFraction, Messages.PFractionDescription, null, typeof(LongFraction));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as LongFraction).Numerator;
        }
    }
}
