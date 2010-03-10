using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the trace of the Hamiltonian matrix of a quantum system
    /// </summary>
    public class HamiltonianMatrixTrace : Fnc {
        public override string Help { get { return Messages.HelpHamiltonianMatrixTrace; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCM system = arguments[0] as LHOQuantumGCM;

            Vector max;
            if(arguments[1] is int) {
                max = new Vector(1);
                max[0] = (int)arguments[1];
            }
            else
                max = arguments[1] as Vector; 
           
            return system.EigenSystem.HamiltonianMatrixTrace(max);
        }
    }
}