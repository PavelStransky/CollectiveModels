using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// For given dynamical system and position in the phase space calculates the energy
    /// </summary>
    public class Energy : FunctionDefinition {
        public override string Help { get { return Messages.HelpEnergy; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PEnergyDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, false, Messages.PPhaseSpacePosition, Messages.PPhaseSpacePositionDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            Vector v = arguments[1] as Vector;
            return dynamicalSystem.E(v);
        }
    }
}
