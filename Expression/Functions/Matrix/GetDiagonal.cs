using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Gets the diagonal of a square matrix
    /// </summary>
    public class GetDiagonal: Fnc {
        public override string Help { get { return Messages.HelpGetDiagonal; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PSquareMatrix, Messages.PSquareMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int l = m.Length;

            Vector result = new Vector(l);
            for(int i = 0; i < l; i++)
                result[i] = m[i, i];

           return result;
        }
    }
}