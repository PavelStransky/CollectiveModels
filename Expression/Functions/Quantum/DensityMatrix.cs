using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vrátí matici hustot vlastní funkce
    /// </summary>
    public class DensityMatrix : Fnc {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(IQuantumSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int), typeof(TArray));

            int count = evaluatedArguments.Count;
            for(int i = 2; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem qs = arguments[0] as IQuantumSystem;

            int[] n;
            bool one = false;

            if(arguments[1] is int) {
                n = new int[1];
                n[0] = (int)arguments[1];

                one = true;
            }
            else {
                TArray a = arguments[1] as TArray;

                int num = a.Length;
                n = new int[num];

                for(int j = 0; j < num; j++)
                    n[j] = (int)a[j];
            }

            int count = arguments.Count;
            Vector[] interval = new Vector[count - 2];
            for(int i = 2; i < count; i++)
                interval[i - 2] = arguments[i] as Vector;

            Matrix[] result = qs.DensityMatrix(n, guider, interval);

            if(one)
                return result[0];
            else
                return new TArray(result);
        }

        private const string help = "Vrátí matici hustot vlastní funkce";
        private const string parameters = "IQuantumSystem; èíslo vlastní funkce (int); oblast výpoètu (Vector, prvky (minx, maxx, numx, ...))";
    }
}
