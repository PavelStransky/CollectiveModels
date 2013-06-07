using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Singular Value Decomposition
    /// </summary>
    public class SVD : Fnc {
        public override string Help { get { return Messages.HelpSVD; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.P2CM, Messages.P2CMDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            bool normalize = (bool)arguments[1];

            if(normalize) {
                Matrix m1 = new Matrix(m.LengthX, m.LengthY);

                for(int i = 0; i < m.LengthX; i++) {
                    Vector v = m.GetRowVector(i);
                    double mean = v.Mean();
                    double var = v.Variance();

                    for(int j = 0; j < m.LengthY; j++)
                        m1[i, j] = (m[i, j] - mean) / var;
                }

                m = m1;
            }

            int mindim = System.Math.Min(m.LengthX, m.LengthY);

            Matrix u = new Matrix(m.LengthX, mindim);
            Matrix vt = new Matrix(mindim, m.LengthY);
            Vector s = new Vector(mindim);

            LAPackDLL.dgesvd(m, u, s, vt);

            List result = new List();
            result.Add(u);
            result.Add(s);
            result.Add(vt);
            return result;
        }
    }
}
