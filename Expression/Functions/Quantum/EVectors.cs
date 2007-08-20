using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí vlastní vektory
    /// </summary>
    public class EVectors : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(IQuantumSystem));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem item = arguments[0] as IQuantumSystem;

            int numEV = item.NumEV;
            TArray result = new TArray(typeof(Vector), numEV);
            for(int i = 0; i < numEV; i++)
                result[i] = item.GetEigenVector(i);

            return result;
        }

        private const string help = "Vrátí vypoèítané vlastní vektory kvantového systému";
        private const string parameters = "QuantumSystem";
    }
}
