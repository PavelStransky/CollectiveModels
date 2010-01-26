using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Distinguishes using SALI whether the trajectory is regular (1) or chaotic (0)
    /// </summary>
    public class IsRegularTrajectory: Fnc {
        public override string Help { get { return Messages.HelpIsRegularTrajectory; } }

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.P2Poincare, Messages.P2PoincareDescription, null, typeof(Vector), typeof(double));
            this.SetParam(2, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(3, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(4, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, string.Empty, typeof(string));
            this.SetParam(5, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
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

            RungeKuttaMethods rkMethodT = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[2], true);
            double precisionT = (double)arguments[3];

            RungeKuttaMethods rkMethodW =
                (string)arguments[4] == string.Empty
                ? rkMethodT
                : (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[4], true);
            double precisionW =
                (double)arguments[5] <= 0.0
                ? precisionT
                : (double)arguments[5];

            SALI sali = new SALI(dynamicalSystem, precisionT, rkMethodT, precisionW, rkMethodW);

            if(sali.IsRegular(ic, guider))
                return 1;
            else
                return 0;
        }
    }
}