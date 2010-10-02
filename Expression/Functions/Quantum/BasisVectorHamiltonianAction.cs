using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Action of Hamiltonian a given basis vector
    /// </summary>
    public class BasisVectorHamiltonianAction: Fnc {
        public override string Help { get { return Messages.HelpBasisVectorHamiltonianAction; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(Vector));
            this.SetParam(2, false, true, false, Messages.PSquared, Messages.PSquaredDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];
            Vector basisIndex = (Vector)arguments[1];
            bool squared = (bool)arguments[2];

            return system.EigenSystem.BasisVectorHamiltonianAction(basisIndex, squared);
        }
    }
}