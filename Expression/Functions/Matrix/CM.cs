using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Calculates correlation matrix
	/// </summary>
	public class CM: Fnc {
		public override string Help {get {return Messages.HelpCM;}}

        protected override void CreateParameters() {
            this.SetNumParams(3);
            
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.P2CM, Messages.P2CMDescription, true, typeof(bool));
            this.SetParam(2, false, true, false, Messages.PShift, Messages.PShiftDescription, 0, typeof(int));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            bool normalize = (bool)arguments[1];
            int shift = (int)arguments[2];

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

            Matrix result = new Matrix(lengthX);

            if(normalize) {
                Matrix m1 = new Matrix(lengthX, lengthY);

                for(int i = 0; i < lengthX; i++) {
                    Vector v = m.GetRowVector(i);
                    double mean = v.Mean();
                    double var = v.Variance();

                    for(int j = 0; j < lengthY; j++)
                        m1[i, j] = (m[i, j] - mean) / var;
                }

                m = m1;
            }

            for(int i = 0; i < lengthX; i++) {
                for(int j = 0; j <= i; j++) {
                    for(int k = 0; k < lengthY - shift; k++)
                        result[i, j] += (m[i, k] * m[j, k + shift] + m[i, k + shift] * m[j, k]) * 0.5;
                    result[i, j] /= lengthY - shift;
                    result[j, i] = result[i, j];
                }
            }

            return result;
        }
	}
}
