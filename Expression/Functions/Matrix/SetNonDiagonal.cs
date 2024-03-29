using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Given value put onto nondiagonal elements of square matrix
    /// </summary>
    public class SetNonDiagonal: Fnc {
        public override string Help { get { return Messages.HelpSetNonDiagonal; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PSquareMatrix, Messages.PSquareMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, true, Messages.PNonDiagonalValue, Messages.PNonDiagonalValueDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int l = m.Length;
            double d = (double)arguments[1];

            for(int i = 0; i < l; i++)
                for(int j = 0; j < l; j++)
                    if(i != j)
                        m[i, j] = d;

            return m;
        }
    }
}