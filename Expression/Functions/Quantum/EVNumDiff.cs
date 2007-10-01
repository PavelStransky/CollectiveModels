using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Pro LHOQuantumGCM tøídu a zadaný vlastní vektor vytvoøí matici H|n> - E|n>
    /// </summary>
    public class EVNumDiff : Fnc {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(LHOQuantumGCMIC));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));

            for(int i = 2; i < evaluatedArguments.Count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCMIC item = arguments[0] as LHOQuantumGCMIC;
            int n = (int)arguments[1];

            Vector[] interval = new Vector[arguments.Count - 2];
            for(int i = 2; i < arguments.Count; i++)
                interval[i - 2] = arguments[i] as Vector;

            return item.NumericalDiff(n, interval);
        }

        private const string help = "Pro LHOQuantumGCM tøídu a zadaný vlastní vektor vytvoøí matici H|n> - E|n>";
        private const string parameters = "LHOQuantumGCM; èíslo vlastní funkce (int); oblast výpoètu (Vector, prvky (minx, maxx, numx, ...))";
    }
}