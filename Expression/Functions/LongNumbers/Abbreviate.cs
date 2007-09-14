using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Abbreviates a fraction
    /// </summary>
    public class Abbreviate: Fnc {
        public override string Help { get { return Messages.HelpAbbreviate; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PFraction, Messages.PFractionDescription, null, typeof(LongFraction));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LongFraction fraction = arguments[0] as LongFraction;
            fraction.Abbreviate();
            return fraction;
        }
    }
}
