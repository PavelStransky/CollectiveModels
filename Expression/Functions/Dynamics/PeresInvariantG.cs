using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates matrix with Poincaré section by the plane y = 0 for 2D system; 
    /// contours are determined by time averaged Peres invariant
    /// </summary>
    public class PeresInvariantG: Fnc {
        public override string Help { get { return Messages.HelpPeresInvariantG; } }

        protected override void CreateParameters() {
            this.SetNumParams(7);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PTime, Messages.PTimeDescription, null, typeof(double));
            this.SetParam(3, true, true, false, Messages.PSizeX, Messages.PSizeXDescription, null, typeof(int));
            this.SetParam(4, true, true, false, Messages.PSizeY, Messages.PSizeYDescription, null, typeof(int));
            this.SetParam(5, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(6, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            double e = (double)arguments[1];
            double time = (double)arguments[2];
            int sizex = (int)arguments[3];
            int sizey = (int)arguments[4];
            RungeKuttaMethods rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[5], true);
            double precision = (double)arguments[6];

            AverageInvariant invariant = new AverageInvariant(dynamicalSystem, precision, rkMethod);
            ArrayList a = invariant.Compute(e, time, sizex, sizey, guider);

            List result = new List();
            result.AddRange(a);

            return result;
        }
    }
}
