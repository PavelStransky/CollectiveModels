using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates a temperature using expression Tr(Ro H) = K
    /// </summary>
    public class QuantumTemperature: Fnc {
        public override string Help { get { return Messages.HelpQuantumTemperature; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, true, Messages.PMeanEnergy, Messages.PMeanEnergyDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];
            double e = (double)arguments[1];

            return system.EigenSystem.QuantumTemperature(e);
        }
    }
}