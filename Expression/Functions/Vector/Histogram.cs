using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Returns the histogram of a vector (and given interval)
    /// </summary>
    public class Histogram: FunctionDefinition {
        public override string Help { get { return Messages.HelpHistogram; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.P1Histogram, Messages.P1HistogramDescription, null,
                typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];

            if(arguments[1] is int)
                return v.Histogram((int)arguments[1], v.Min(), v.Max());
            else if(arguments[1] is Vector)
                return v.Histogram((Vector)arguments[1]);

            return null;
        }
    }
}
