using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Transforms the matrix with indices (Z, A) to (Z, N)
    /// </summary>
    public class MatrixZAToNA: Fnc {
        public override string Help { get { return Messages.HelpMatrixZAToNA; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = arguments[0] as Matrix;
            int lengthZ = m.LengthX;
            int lengthA = m.LengthY;

            int lengthN = lengthA - lengthZ;
            Matrix result = new Matrix(lengthZ, lengthN);

            for(int i = 0; i < lengthZ; i++)
                for(int j = 0; j < lengthN; j++)
                    result[i, j] = m[i, i + j];

            return result;
        }
    }
}
