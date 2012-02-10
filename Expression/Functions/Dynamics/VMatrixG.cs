using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates matrix with value of the eigenvalue of cal{V} matrix
    /// </summary>
    public class VMatrixG: Fnc {
        public override string Help { get { return Messages.HelpSALIG; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(IGeometricalMethod));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
            this.SetParam(4, false, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IGeometricalMethod gcm = arguments[0] as IGeometricalMethod;

            double e = (double)arguments[1];
            Vector vx = (Vector)arguments[2];
            Vector vy = (Vector)arguments[3];
            int ei = (int)arguments[4];

            int lengthX = (int)vx[2];
            int lengthY = (int)vy[2];

            double minx = vx[0]; double maxx = vx[1]; double koefx = (maxx - minx) / (lengthX - 1);
            double miny = vy[0]; double maxy = vy[1]; double koefy = (maxy - miny) / (lengthY - 1);

            Matrix result = new Matrix(lengthX, lengthY);

            for(int i = 0; i < lengthX; i++) {
                double x = i * koefx + minx;
                for(int j = 0; j < lengthY; j++) {
                    double y = j * koefy + miny;
                    Matrix m = gcm.VMatrix(e, x, y);
                    Vector ev = LAPackDLL.dsyev(m, false)[0];
                    result[i, j] = ev[ei];
                }
            }

            return result;
        }
    }
}
