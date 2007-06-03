using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice vybere zadané sloupce
	/// </summary>
	public class GetColumns: FunctionDefinition {
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
            Vector columnsToGet = (Vector)arguments[1];

            int lengthX = m.LengthX;
            int n = columnsToGet.Length;

			Matrix result = new Matrix(lengthX, n);
			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < n; j++)
					result[i, j] = m[i, (int)columnsToGet[j]];
			return result;
		}

		private const string help = "Z matice vybere zadané sloupce";
		private const string parameters = "Matrix; indexy sloupcù";
	}
}
