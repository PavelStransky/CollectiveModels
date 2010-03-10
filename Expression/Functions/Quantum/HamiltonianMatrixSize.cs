using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Dimensions of the Hamiltonian matrix
    /// </summary>
    public class HamiltonianMatrixSize : Fnc {
        public override string Help { get { return Messages.HelpHamiltonianMatrixSize; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = arguments[0] as IQuantumSystem;

            Vector max;
            if(arguments[1] is int) {
                max = new Vector(1);
                max[0] = (int)arguments[1];
            }
            else
                max = arguments[1] as Vector;

            return system.EigenSystem.HamiltonianMatrixSize(max);
        }
    }
}