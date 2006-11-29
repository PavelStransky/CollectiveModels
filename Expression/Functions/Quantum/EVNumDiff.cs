using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro LHOQuantumGCM tøídu a zadaný vlastní vektor vytvoøí matici H|n> - E|n>
    /// </summary>
    public class EVNumDiff : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 3);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(Vector));
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is LHOQuantumGCM) {
                LHOQuantumGCM qs = item as LHOQuantumGCM;
                int n = (int)arguments[1];
                Vector range = arguments[2] as Vector;

                return qs.NumericalDiff(n, range);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Pro LHOQuantumGCM tøídu a zadaný vlastní vektor vytvoøí matici H|n> - E|n>";
        private const string parameters = "LHOQuantumGCM; èíslo vlastní funkce (int); oblast výpoètu (Vector, prvky (minx, maxx, numx, ...))";
    }
}