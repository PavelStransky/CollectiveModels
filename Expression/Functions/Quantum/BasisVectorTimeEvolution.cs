using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Time evolution of a given basis vector
    /// </summary>
    public class BasisVectorTimeEvolution: Fnc {
        public override string Help { get { return Messages.HelpBasisVectorTimeEvolution; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(Vector));
            this.SetParam(2, true, true, true, Messages.PTime, Messages.PTimeDescription, false, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];

            Vector basisIndex = (Vector)arguments[1];
            double time = (double)arguments[2];

            return system.EigenSystem.TimeEvolution(basisIndex, time);
        }
    }
}