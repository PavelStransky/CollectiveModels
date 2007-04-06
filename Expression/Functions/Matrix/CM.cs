using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá korelaèní matici (matice)
	/// </summary>
	public class CM: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Matrix));
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

            int shift = (arguments.Count > 1) ? (int)arguments[1] : 0;

            Matrix result = new Matrix(lengthX);

            for(int i = 0; i < lengthX; i++) {
                for(int j = 0; j <= i; j++) {
                    result[i, j] = 0;
                    for(int k = 0; k < lengthY - shift; k++)
                        result[i, j] += (m[i, k] * m[j, k + shift] + m[i, k + shift] * m[j, k]) * 0.5;
                    result[i, j] /= lengthY - shift;
                    result[j, i] = result[i, j];
                }
            }

            return result;
        }

		private const string help = "Vypoèítá korelaèní matici ze zadané matice se signálem";
		private const string parameters = "Matrix [;posun (int)]";
	}
}
