using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí stopu Hamiltonovy matice kvantového systému
    /// </summary>
    public class HamiltonianMatrixTrace : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(LHOQuantumGCM));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCM item = arguments[0] as LHOQuantumGCM;
            int maxE = (int)arguments[1];
            int numSteps = 0;               // 0 - numsteps je dopoèítáno automaticky

            if(arguments.Count > 2) 
                numSteps = (int)arguments[2];

            return item.HamiltonianMatrixTrace(maxE, numSteps, guider.Writer);
        }

        private const string help = "Vrátí stopu Hamiltoniánu v použité bázi";
        private const string parameters = "LHOQuantumGCM objekt; MaxE (int); [NumSteps - dìlení møíže (int)]";
    }
}