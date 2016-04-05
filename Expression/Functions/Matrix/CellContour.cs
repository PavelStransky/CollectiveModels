using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns borders of all values bigger than given value of a given matrix
    /// </summary>
    public class CellContour : Fnc {
        public override string Help { get { return Messages.HelpCellContour; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, true, Messages.PValue, Messages.PValueDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            double v = (double)arguments[1];

            ArrayList a = new ArrayList();

            for(int i = 0; i < m.LengthX; i++)
                for(int j = 0; j<m.LengthY;j++)
                    if(m[i, j] > v) {
                        PointVector p = new PointVector(5);
                        p[0] = new PointD(i - 0.5, j - 0.5);
                        p[1] = new PointD(i - 0.5, j + 0.5);
                        p[2] = new PointD(i + 0.5, j + 0.5);
                        p[3] = new PointD(i + 0.5, j - 0.5);
                        p[4] = p[0];
                        a.Add(p);
                    }

            TArray result = new TArray(typeof(PointVector), a.Count);
            int k = 0;
            foreach(PointVector p in a)
                result[k++] = p;

            return result;
        }
    }
}
