using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.PT;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// 
    /// </summary>
    public class Orbit: Fnc {
        public override string Help { get { return string.Empty; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, false, Messages.PInitialCondition, Messages.PInitialConditionDescription, null, typeof(Vector));
            this.SetParam(2, false, true, false, Messages.PPSPoints, Messages.PPSPointsDescription, 0, typeof(int));
            this.SetParam(3, false, true, false, Messages.PMinimumPointsCircle, Messages.PMinimumPointsCircleDescription, 0, typeof(int));
            this.SetParam(4, false, true, false, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            Vector ic = arguments[1] as Vector;
            int psSize = (int)arguments[2];
            int minCircle = (int)arguments[3];
            double precision = (double)arguments[4];

            PeriodicOrbit p = new PeriodicOrbit(dynamicalSystem);
            ArrayList o = p.Compute(ic, psSize, minCircle, precision, guider);

            List result = new List();
            result.AddRange(o);
            return result;
        }
    }
}