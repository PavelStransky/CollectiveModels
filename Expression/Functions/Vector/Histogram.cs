using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the histogram of a vector (and given interval)
    /// </summary>
    public class Histogram: Fnc {
        public override string Help { get { return Messages.HelpHistogram; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector), typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.P1Histogram, Messages.P1HistogramDescription, null,
                typeof(Vector), typeof(int));
            this.SetParam(2, false, true, false, Messages.PHistogramType, Messages.PHistogramTypeDescription, "point", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector.HistogramTypes type = (Vector.HistogramTypes)Enum.Parse(typeof(Vector.HistogramTypes), (string)arguments[2], true);

            if(arguments[0] is Vector) {
                Vector v = (Vector)arguments[0];
                return (arguments[1] is int)
                    ? v.Histogram((int)arguments[1], v.Min(), v.Max(), type)
                    : v.Histogram((Vector)arguments[1], type);
            }
            else {
                PointVector pv = (PointVector)arguments[0];
                return (arguments[1] is int)
                    ? pv.Histogram((int)arguments[1], pv.VectorX.Min(), pv.VectorX.Max(), type)
                    : pv.Histogram((Vector)arguments[1], type);
            }
        }
    }
}
