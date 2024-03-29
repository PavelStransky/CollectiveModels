using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Action of a Hamiltonian on a given ket
    /// </summary>
    public class HamiltonianAction: Fnc {
        public override string Help { get { return Messages.HelpHamiltonianAction; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PKet, Messages.PKetDescription, null, typeof(PointVector), typeof(Vector));
            this.SetParam(2, false, true, false, Messages.PSquared, Messages.PSquaredDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];
            bool squared = (bool)arguments[2];

            if(arguments[1] is Vector)
                return system.EigenSystem.HamiltonianAction(new PointVector(arguments[1] as Vector, 0.0), squared).VectorX;
            else
                return system.EigenSystem.HamiltonianAction(arguments[1] as PointVector, squared);
        }
    }
}