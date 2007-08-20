using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Returns the trace of the Hamiltonian matrix of a quantum system
    /// </summary>
    public class HamiltonianMatrixTrace : FunctionDefinition {
        public override string Help { get { return Messages.HelpHamiltonianMatrixTrace; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, null, typeof(LHOQuantumGCM));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PNumSteps, Messages.PNumStepsDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCM item = arguments[0] as LHOQuantumGCM;
            int maxE = (int)arguments[1];
            int numSteps = (int)arguments[2];               // 0 - numsteps je dopoèítáno automaticky

            return item.HamiltonianMatrixTrace(maxE, numSteps, guider);
        }
    }
}