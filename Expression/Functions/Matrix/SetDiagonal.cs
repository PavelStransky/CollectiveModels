using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Given value or Vector of values put onto diagonal of square matrix
    /// </summary>
    public class SetDiagonal: Fnc {
        public override string Help { get { return Messages.HelpSetDiagonal; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PSquareMatrix, Messages.PSquareMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, true, Messages.PDiagonalValue, Messages.PDiagonalValueDescription, null, typeof(Vector), typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int l = m.Length;

            if(arguments[1] is double) {
                double d = (double)arguments[1];

                for(int i = 0; i < l; i++)
                    m[i, i] = d;
            }

            else if(arguments[1] is Vector) {
                Vector v = (Vector)arguments[1];
                int lv = v.Length;

                if(l != lv)
                    throw new FncException(
                        this,
                        Messages.EMNotEqualLength,
                        string.Format(Messages.EMNotEqualLengthDetail, l, lv));

                for(int i = 0; i < l; i++)
                    m[i, i] = v[i];
            }

            return m;
        }
    }
}