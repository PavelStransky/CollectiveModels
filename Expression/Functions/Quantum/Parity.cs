using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí paritu stavù
    /// </summary>
    public class Parity: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(LHOQuantumGCMR));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as LHOQuantumGCMR).Parity();
        }

        private const string help = "Vrátí paritu stavù";
        private const string parameters = "LHOQuantumGCMR";
    }
}
