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
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is IQuantumSystem) {
                IQuantumSystem q = item as IQuantumSystem;
                TArray result = new TArray();
                for(int i = 0; i < q.NumEV; i++)
                    result.Add(q.GetEigenVector(i));
                return result;
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrátí vypoèítané vlastní vektory kvantového systému";
        private const string parameters = "QuantumSystem";
    }
}
