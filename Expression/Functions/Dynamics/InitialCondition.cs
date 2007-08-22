using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given dynamical system and energy generates initial condition of a trajectory and returns it as Vector
    /// </summary>
    public class InitialCondition: Fnc {
        public override string Help { get { return Messages.HelpInitialCondition; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, true, Messages.P3InitialCondition, Messages.P3InitialConditionDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            double e = (double)arguments[1];

            if(arguments[2] != null) {
                double l = (double)arguments[2];
                return dynamicalSystem.IC(e, l);
            }
            else
                return dynamicalSystem.IC(e);
        }
    }
}
