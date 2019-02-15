using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Reduces the size of a matrix by a given factor
    /// </summary>
    public class ReduceMatrixSize : Fnc {
        public override string Help { get { return Messages.HelpReduceSize; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PFactor, Messages.PFactorXDescription, 1, typeof(int));
            this.SetParam(2, false, true, false, Messages.PFactorY, Messages.PFactorYDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = arguments[0] as Matrix;
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            int factorX = (int)(arguments[1]);
            int factorY = (int)(arguments[2]);
            if(factorY <= 0) 
                factorY = factorX;

            int lx = lengthX / factorX;
            int ly = lengthY / factorY;

            Matrix result = new Matrix(lx, ly);

            for(int i = 0; i < lx; i++)
                for(int j = 0; j < ly; j++) {
                    int num = 0;
                    int ix = factorX * i;
                    int iy = factorY * j;
                    for(int k = 0; k < factorX; k++)
                        for(int l = 0; l < factorY; l++)
                            if(ix + k < lengthX && iy + l < lengthY) {
                                result[i, j] += m[ix + k, iy + l];
                                num++;
                            }
                    if(num > 0)
                        result[i, j] /= num;
                }

            return result;
        }
    }
}