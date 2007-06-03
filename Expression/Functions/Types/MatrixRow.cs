using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vektory slou�� do ��dk� matice
    /// </summary>
    public class MatrixRow : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);

            int count = evaluatedArguments.Count;

            for(int i = 0; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int length = (arguments[0] as Vector).Length;

            Matrix result = new Matrix(count, length);

            for(int i = 0; i < count; i++)
                result.SetRowVector(i, arguments[i] as Vector);

            return result;
        }

        private const string name = "MatrixRow";
        private const string help = "Vektor� slou�� do ��dk� matice (Matrix)";
        private const string parameters = "vektor 1 [; vektor 2...]";
    }
}
