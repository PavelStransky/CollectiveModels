using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given dynamical system and position in the phase space calculates the energy
    /// </summary>
    public class Energy : Fnc {
        public override string Help { get { return Messages.HelpEnergy; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PEnergyDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, false, Messages.PPhaseSpacePosition, Messages.PPhaseSpacePositionDescription, null, typeof(Vector), typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            Vector v = null;
            if (arguments[1] is Vector)
                v = arguments[1] as Vector;
            else if (arguments[1] is double) {
                v = new Vector(2 * dynamicalSystem.DegreesOfFreedom);
                v[0] = (double)arguments[1];
            }

            return dynamicalSystem.E(v);
        }
    }
}
