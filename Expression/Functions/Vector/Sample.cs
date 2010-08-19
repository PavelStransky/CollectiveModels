using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Samples the pointvector in the points of the given vector
    /// </summary>
    public class Sample: Fnc {
        public override string Help { get { return Messages.HelpSample; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PSamplingPoints, Messages.PSamplingPointsDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv = arguments[0] as PointVector;
            Vector points = arguments[1] as Vector;

            return pv.Sample(points);
        }
    }
}