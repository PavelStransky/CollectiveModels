using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí matici hustot vlastní funkce
    /// </summary>
    public class DensityMatrix : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

            for(int i = 2; i < evaluatedArguments.Count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, typeof(Vector));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is IQuantumSystem) {
                IQuantumSystem qs = item as IQuantumSystem;
                int n = (int)arguments[1];

                Vector[] interval = new Vector[arguments.Count - 2];
                for(int i = 2; i < arguments.Count; i++)
                    interval[i - 2] = arguments[i] as Vector;

                return qs.DensityMatrix(n, interval);
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrátí matici hustot vlastní funkce";
        private const string parameters = "QuantumSystem; èíslo vlastní funkce (int); oblast výpoètu (Vector, prvky (minx, maxx, numx, ...))";
    }
}
