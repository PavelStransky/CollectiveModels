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

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(IGeometricalMethod), typeof(ClassicalCW), typeof(GeometricCounterExample));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
            this.SetParam(4, false, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double e = (double)arguments[1];
            Vector vx = (Vector)arguments[2];
            Vector vy = (Vector)arguments[3];
            int ei = (int)arguments[4];

            int lengthX = (int)vx[2];
            int lengthY = (int)vy[2];

            double minx = vx[0]; double maxx = vx[1]; double koefx = lengthX == 1 ? 0 : (maxx - minx) / (lengthX - 1);
            double miny = vy[0]; double maxy = vy[1]; double koefy = lengthY == 1 ? 0 : (maxy - miny) / (lengthY - 1);

            Matrix result = new Matrix(lengthX, lengthY);

            for(int i = 0; i < lengthX; i++) {
                double x = i * koefx + minx;
                for(int j = 0; j < lengthY; j++) {
                    double y = j * koefy + miny;

                    // Potential
                    if(ei >= 0) {
                        Matrix m = null;
                        if(arguments[0] is ClassicalGCM) {
                            if(ei < 2)
                                m = (arguments[0] as ClassicalGCM).VMatrix(e, x, y);
                            else 
                                m = (arguments[0] as ClassicalGCM).MMatrix(e, x, y);                            
                        }
                        else if(arguments[0] is ClassicalCW)
                            m = (arguments[0] as ClassicalCW).VMatrix(e, x, y);
                        else if(arguments[0] is GeometricCounterExample)
                            m = (arguments[0] as GeometricCounterExample).VMatrix(e, x, y);

                        Vector ev = LAPackDLL.dsyev(m, false)[0];

                        if(arguments[0] is ClassicalGCM)
                            result[i, j] = ev[ei % 2];
                        else
                            result[i, j] = ev[ei];
                    }

                    else {
                        if(arguments[0] is ClassicalCW)
                            result[i, j] = e - (arguments[0] as ClassicalCW).V(x, y);
                        else if(arguments[0] is GeometricCounterExample)
                            result[i, j] = e - (arguments[0] as GeometricCounterExample).V(x, y);
                        else if(arguments[0] is ClassicalGCM)
                            result[i, j] = e - (arguments[0] as ClassicalGCM).V(x, y);
                    }
                }
            }

            return result;
        }
    }
}
