using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vektory slouèí do sloupcù matice
    /// </summary>
    public class MatrixColumn: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);

            int count = evaluatedArguments.Count;

            for(int i = 0; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int length = (arguments[0] as Vector).Length;

            Matrix result = new Matrix(length, count);

            for(int i = 0; i < count; i++)
                result.SetColumnVector(i, arguments[i] as Vector);

            return result;
        }

        private const string name = "MatrixColumn";
        private const string help = "Vektorù slouèí do sloupcù matice (Matrix)";
        private const string parameters = "vektor 1 [; vektor 2...]";
    }
}
