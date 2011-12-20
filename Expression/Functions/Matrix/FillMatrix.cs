using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Fills a matrix with points that correspond to a given PointVector
    /// </summary>
    public class FillMatrix: Fnc {
        public override string Help { get { return Messages.HelpFillMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(5, true);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(2, true, true, true, Messages.PValue, Messages.PValueDescription, 0.0, typeof(double));
            this.SetParam(3, true, true, false, Messages.PBounds, Messages.PBoundsDescription, null, typeof(Vector));
            this.SetParam(4, false, true, false, Messages.P5FillMatrix, Messages.P5FillMatrixDescription, "keep", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)((Matrix)arguments[0]).Clone();
            PointVector pv = (PointVector)arguments[1];
            double d = (double)arguments[2];
            Vector bounds = (Vector)arguments[3];

            bool overwrite = ((string)arguments[4]).Trim().ToLower() == "overwrite";

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            int length = pv.Length;

            double addX = -bounds[0];
            double addY = -bounds[2];
            double koefX = lengthX / (bounds[1] - bounds[0]);
            double koefY = lengthY / (bounds[3] - bounds[2]);

            for(int k = 0; k < length; k++) {
                int i = (int)((pv[k].X + addX) * koefX);
                int j = (int)((pv[k].Y + addY) * koefY);

                if(i >= 0 && i < lengthX && j >= 0 && j < lengthY)
                    if(m[i, j] == 0 || overwrite)
                        m[i, j] = d;
            }

            return m;
        }
    }
}
