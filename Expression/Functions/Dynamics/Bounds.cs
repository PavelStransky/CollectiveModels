using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// For given dynamical system and energy determines the bounds (higher limit) in which the solution can be found
    /// </summary>
    public class Bounds : FunctionDefinition {
        public override string Help { get { return Messages.HelpBounds; } }

        protected override void CreateParameters() {
            this.NumParams(2);
            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            return dynamicalSystem.Bounds((double)arguments[1]);
        }
    }
}
