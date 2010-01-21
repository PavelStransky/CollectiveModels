using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates mean of a PointVector in each of a defined bin
    /// </summary>
    public class BinMean: Fnc {
        public override string Help { get { return Messages.HelpBinMean; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.P1Histogram, Messages.P1HistogramDescription, null,
                typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv = arguments[0] as PointVector;

            return (arguments[1] is int)
                ? pv.BinMean((int)arguments[1], pv.MinX(), pv.MaxX())
                : pv.BinMean((Vector)arguments[1]);
        }
    }
}
