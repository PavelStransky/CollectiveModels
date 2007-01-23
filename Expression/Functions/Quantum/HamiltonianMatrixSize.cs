using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí velikost Hamiltonovy matice
    /// </summary>
    public class HamiltonianMatrixSize : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as LHOQuantumGCM != null) {
                int maxE = (int)arguments[1];
                return (item as LHOQuantumGCM).HamiltonianMatrixSize(maxE);
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrátí rozmìry matice Hamiltoniánu v použité bázi";
        private const string parameters = "LHOQuantumGCM objekt; MaxE (int)";
    }
}