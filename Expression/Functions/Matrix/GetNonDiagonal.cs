using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Get the upper nondiagonal elements of a square matrix and returns it as a vector
    /// </summary>
    public class GetNonDiagonal : Fnc {
        public override string Help { get { return Messages.HelpGetNonDiagonal; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PSquareMatrix, Messages.PSquareMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int l = m.Length;

            Vector result = new Vector(l * (l - 1) / 2);

            int k = 0;
            for(int i = 0; i < l; i++)
                for(int j = i + 1; j < l; j++)
                    result[k++] = m[i, j];

            return result;
        }
    }
}