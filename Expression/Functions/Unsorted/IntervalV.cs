using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates points for interval
    /// </summary>
    public class IntervalV: Fnc {
        public override string Help { get { return Messages.HelpIntervalV; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, true, Messages.PStartingPoint, Messages.PStartingPointDetail, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.PEndingPoint, Messages.PEndingPointDetail, null, typeof(double));
            this.SetParam(2, true, true, false, Messages.PNumberOfPoints, Messages.PNumberOfPointsDetail, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int num = (int)arguments[2];
            DiscreteInterval di = new DiscreteInterval((double)arguments[0], (double)arguments[1], num);
            Vector v = new Vector(num);

            for(int i = 0; i < num; i++)
                v[i] = di.GetX(i);

            return v;
        }
    }
}
