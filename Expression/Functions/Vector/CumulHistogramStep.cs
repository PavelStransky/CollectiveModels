using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an exact cumulative histogram as a step function
    /// </summary>
    public class CumulHistogramStep: Fnc {
        public override string Help { get { return Messages.HelpCumulHistogramStep; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];
            return v.CumulativeHistogramStep();
        }
    }
}
