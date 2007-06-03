using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice vybere zadané øádky
	/// </summary>
	public class GetRows: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(Matrix));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));

            this.ConvertArray2Vectors(evaluatedArguments, 1);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            Vector rowsToGet = (Vector)arguments[1];

            int lengthY = m.LengthY;
            int n = rowsToGet.Length;

            Matrix result = new Matrix(n, lengthY);
            for(int i = 0; i < n; i++)
                for(int j = 0; j < lengthY; j++)
                    result[i, j] = m[(int)rowsToGet[i], j];
            return result;
        }

		private const string help = "Z matice vybere zadané øádky";
		private const string parameters = "Matrix; indexy øádkù";
	}
}
