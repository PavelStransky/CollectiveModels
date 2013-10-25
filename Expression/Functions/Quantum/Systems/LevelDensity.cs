using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Semiclassical level density
    /// </summary>
    public class LevelDensity : Fnc {
        public override string Help { get { return Messages.HelpLevelDensity; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(QuantumCW), typeof(PT3));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, true, Messages.PStep, Messages.PStepDescription, 1E-5, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double e = (double)arguments[1];
            double step = (double)arguments[2];

            if(arguments[0] is QuantumCW)
                return (arguments[0] as QuantumCW).LevelDensity(e, step);
            else if(arguments[0] is PT3)
                return (arguments[0] as PT3).LevelDensity(e, step);

            return null;
        }
    }
}
