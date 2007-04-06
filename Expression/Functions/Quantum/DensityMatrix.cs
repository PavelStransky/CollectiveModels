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

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IQuantumSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

            int count = evaluatedArguments.Count;
            for(int i = 2; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem qs = arguments[0] as IQuantumSystem;
            int n = (int)arguments[1];

            int count = arguments.Count;
            Vector[] interval = new Vector[count - 2];
            for(int i = 2; i < count; i++)
                interval[i - 2] = arguments[i] as Vector;

            return qs.DensityMatrix(n, interval);
        }

        private const string help = "Vrátí matici hustot vlastní funkce";
        private const string parameters = "IQuantumSystem; èíslo vlastní funkce (int); oblast výpoètu (Vector, prvky (minx, maxx, numx, ...))";
    }
}
