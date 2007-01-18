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
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

            if(evaluatedArguments.Count > 2)
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as LHOQuantumGCM != null) {
                int maxE = (int)arguments[1];
                int numSteps = 0;               // 0 - numsteps je dopoèítáno automaticky

                if(arguments.Count > 2) {
                    numSteps = (int)arguments[2];
                }

                return (item as LHOQuantumGCM).HamiltonianMatrix(maxE, numSteps, this.writer);
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí matici Hamiltoniánu v použité bázi";
        private const string parameters = "LHOQuantumGCM objekt; MaxE (int); [NumSteps - dìlení møíže (int)]";
    }
}