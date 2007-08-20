using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z matice odstraní zadané øádky
	/// </summary>
	public class DropRows: FunctionDefinition {
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
            Vector rowsToDrop = (Vector)arguments[1];

            rowsToDrop = (Vector)rowsToDrop.Sort();
            rowsToDrop = rowsToDrop.RemoveDuplicity();

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            int n = rowsToDrop.Length;

            Matrix result = new Matrix(lengthX - n, lengthY);
            int index = 0;
            for(int i = 0; i < lengthX; i++)
                if(rowsToDrop.Find(i) < 0) {
                    for(int j = 0; j < lengthY; j++)
                        result[index, j] = m[i, j];
                    index++;
                }
            return result;	
		}

		private const string help = "Z matice odstraní zadané øádky";
		private const string parameters = "Matrix; indexy øádkù";
	}
}
