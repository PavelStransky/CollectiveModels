using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the dependence of classical Peres invariant on time for given trajectory
    /// </summary>
    public class PeresInvariantT: Fnc {
        public override string Help { get { return Messages.HelpSALI; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.P2Poincare, Messages.P2PoincareDescription, null, typeof(Vector), typeof(double));
            this.SetParam(2, true, true, true, Messages.PTime, Messages.PTimeDescription, null, typeof(double));
            this.SetParam(3, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(4, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            Vector ic;

            // Trajectory is generated randomly
            if(arguments[1] is double) {
                double e = (double)arguments[1];
                ic = dynamicalSystem.IC(e);
            }
            else if(arguments[1] is Vector && (arguments[1] as Vector).Length / 2 == dynamicalSystem.DegreesOfFreedom)
                ic = (Vector)arguments[1];
            else
                return this.BadTypeError(arguments[1], 1);

            double time = (double)arguments[2];
            RungeKuttaMethods rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[3], true);
            double precision = (double)arguments[4];

            AverageInvariant ai = new AverageInvariant(dynamicalSystem, precision, rkMethod);
            return ai.TimeDependence(ic, time);
        }
    }
}
