using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí Hamiltonovu matici kvantového systému
    /// </summary>
    public class HamiltonianMatrix : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(LHOQuantumGCM));

            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, evaluateArray, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCM item = arguments[0] as LHOQuantumGCM;
            int maxE = (int)arguments[1];
            int numSteps = 0;               // 0 - numsteps je dopoèítáno automaticky

            if(arguments.Count > 2) 
                numSteps = (int)arguments[2];

            return item.HamiltonianMatrix(maxE, numSteps, guider);
        }

        private const string help = "Vytvoøí matici Hamiltoniánu v použité bázi";
        private const string parameters = "LHOQuantumGCM objekt; MaxE (int); [NumSteps - dìlení møíže (int)]";
    }
}