using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns Hamiltonian matrix of the given quantum system
    /// </summary>
    public class HamiltonianMatrix : Fnc {
        public override string Help { get { return Messages.HelpHamiltonianMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(LHOQuantumGCM), typeof(DoublePendulum));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PNumberOfPoints, Messages.PNumberOfPointsDetail, 0, typeof(int));
        }


        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int maxE = (int)arguments[1];
            int numSteps = (int)arguments[2];

            if(arguments[0] is LHOQuantumGCM)
                return (arguments[0] as LHOQuantumGCM).HamiltonianMatrix(maxE, numSteps, guider);
            else
                return (arguments[0] as DoublePendulum).HamiltonianMatrix(maxE, numSteps, guider);
        }
    }
}