using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns true if the Poincare section on the given energy is fully regular; otherwise returns false
    /// </summary>
    public class IsRegularPS: Fnc {
        public override string Help { get { return Messages.HelpIsRegularPS; } }

        protected override void CreateParameters() {
            this.SetNumParams(9);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, false, Messages.PSizeX, Messages.PSizeXDescription, null, typeof(int));
            this.SetParam(3, true, true, false, Messages.PSizeY, Messages.PSizeYDescription, null, typeof(int));
            this.SetParam(4, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(5, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(6, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, string.Empty, typeof(string));
            this.SetParam(7, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(8, false, true, false, Messages.PXSection, Messages.PXSectionDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            double e = (double)arguments[1];
            int sizex = (int)arguments[2];
            int sizey = (int)arguments[3];
            RungeKuttaMethods rkMethodT = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[4], true);
            double precisionT = (double)arguments[5];

            RungeKuttaMethods rkMethodW =
                (string)arguments[6] == string.Empty
                ? rkMethodT
                : (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[6], true);
            double precisionW =
                (double)arguments[7] <= 0.0
                ? precisionT
                : (double)arguments[7];

            bool isX = (bool)arguments[8];

            SALIContourGraph sali = new SALIContourGraph(dynamicalSystem, precisionT, rkMethodT, precisionW, rkMethodW);
            return sali.IsRegularGraph(e, sizex, sizey, isX, guider);
        }
    }
}
