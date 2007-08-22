using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Z matice odstraní zadané sloupce
	/// </summary>
	public class DropColumns: Fnc {
		public override string Help {get {return help;}}
		public override string ParametersHelp {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(Matrix));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));

            this.ConvertArray2Vectors(evaluatedArguments, 1);
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            Vector columnsToDrop = (Vector)arguments[1];

            columnsToDrop = (Vector)columnsToDrop.Sort();
            columnsToDrop = columnsToDrop.RemoveDuplicity();

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            int n = columnsToDrop.Length;

            Matrix result = new Matrix(lengthX, lengthY - n);
            int index = 0;
            for(int j = 0; j < lengthY; j++)
                if(columnsToDrop.Find(j) < 0) {
                    for(int i = 0; i < lengthX; i++)
                        result[i, index] = m[i, j];
                    index++;
                }
            return result;
        }

		private const string help = "Z matice odstraní zadané sloupce";
		private const string parameters = "Matrix; indexy sloupcù";
	}
}
