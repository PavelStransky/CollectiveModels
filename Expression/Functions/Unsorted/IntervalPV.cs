using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates points for interval (as pointvector)
    /// </summary>
    public class IntervalPV: IntervalV {
        public override string Help { get { return Messages.HelpIntervalPV; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = base.EvaluateFn(guider, arguments) as Vector;
            return new PointVector(v, new Vector(v.Length));
        }
    }
}
