using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates SALI dependence on time for given trajectory
    /// </summary>
    public class FnSALI: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpSALI; } }

        protected override void CreateParameters() {
            this.SetNumParams(8);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.P2Poincare, Messages.P2PoincareDescription, null, typeof(Vector), typeof(double));
            this.SetParam(2, true, true, true, Messages.PTime, Messages.PTimeDescription, null, typeof(double));
            this.SetParam(3, false, true, true, Messages.PTimeStep, Messages.PTimeStepDescription, 0.0, typeof(double));
            this.SetParam(4, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(5, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-3, typeof(double));
            this.SetParam(6, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(7, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-3, typeof(double));
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
            double timeStep = (double)arguments[3];
            RungeKuttaMethods rkMethodT = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[4], true);
            double precisionT = (double)arguments[5];
            RungeKuttaMethods rkMethodW = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[6], true);
            double precisionW = (double)arguments[7];

            SALI sali = new SALI(dynamicalSystem, precisionT, rkMethodT, precisionW, rkMethodW);
            return sali.TimeDependence(ic, time, timeStep);
        }

        private const string name = "sali";
    }
}
