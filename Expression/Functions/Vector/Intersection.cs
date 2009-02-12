using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds all intersection points of two pointvectors
    /// </summary>
    public class Intersection: Fnc {
        public override string Help { get { return Messages.HelpIntersection; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv1 = arguments[0] as PointVector;
            PointVector pv2 = arguments[1] as PointVector;

            return pv1.Intersection(pv2);
        }
    }
}