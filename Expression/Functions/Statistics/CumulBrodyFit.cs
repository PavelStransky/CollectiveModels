using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the best Brody fit according to chi square test
    /// </summary>
    public class CumulBrodyFit: Fnc {
        public override string Help { get { return Messages.HelpCumulBrodyFit; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.P1CumulBrodyFit, Messages.P1CumulBrodyFitDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, true, Messages.PPrecision, Messages.PPrecision, 1.0E-3, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv = (PointVector)arguments[0];

            BrodyFit b = new BrodyFit(pv);
            return b.Fit((double)arguments[1]);
        }
    }
}
