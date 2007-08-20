using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro LHOQuantumGCM t��du a zadan� vlastn� vektor vytvo�� matici H|n> - E|n>
    /// </summary>
    public class EVNumDiff : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(LHOQuantumGCMC));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));

            for(int i = 2; i < evaluatedArguments.Count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCMC item = arguments[0] as LHOQuantumGCMC;
            int n = (int)arguments[1];

            Vector[] interval = new Vector[arguments.Count - 2];
            for(int i = 2; i < arguments.Count; i++)
                interval[i - 2] = arguments[i] as Vector;

            return item.NumericalDiff(n, interval);
        }

        private const string help = "Pro LHOQuantumGCM t��du a zadan� vlastn� vektor vytvo�� matici H|n> - E|n>";
        private const string parameters = "LHOQuantumGCM; ��slo vlastn� funkce (int); oblast v�po�tu (Vector, prvky (minx, maxx, numx, ...))";
    }
}