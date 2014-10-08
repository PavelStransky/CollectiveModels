using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Interpolates a 3D function on a given set of points
    /// </summary>
    public class Interpolate3D : Fnc {
        public override string Help { get { return Messages.HelpInterpolate3D; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PZ, Messages.PZDescription, null, typeof(Vector));
            this.SetParam(2, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(3, true, true, false, Messages.PRadius, Messages.PRadiusDescription, null, typeof(double));
            this.SetParam(4, true, true, false, Messages.PMinimumPointsCircle, Messages.PMinimumPointsCircleDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector xy = (PointVector)arguments[0];
            Vector z = (Vector)arguments[1];
            int length = z.Length;

            PointVector xynew = (PointVector)arguments[2];
            int lengthnew = xynew.Length;
            Vector znew = new Vector(lengthnew);

            double r = (double)arguments[3];
            int min = (int)arguments[4];
            if(min < 1)
                min = 1;

            for(int i = 0; i < lengthnew; i++) {
                double s = 0.0;
                double w = 0.0;

                int[] jm = new int[min];
                double[] dm = new double[min];

                dm[0] = 1.0 / eps;

                int num = 0;

                for(int j = 0; j < length; j++) {
                    double dist = PointD.Distance(xynew[i], xy[j]);

                    if(dist < dm[0]) {
                        for(int k = 1; k < min; k++) {
                            dm[k] = dm[k - 1];
                            jm[k] = jm[k - 1];
                        }
                        dm[0] = dist;
                        jm[0] = j;
                    }

                    if(dist <= r) {
                        if(dist == 0)
                            dist = eps;
                        s += z[j] / dist;
                        w += 1.0 / dist;

                        num++;
                    }
                }

                if(num < min) {
                    for(int k = 0; k < min; k++) {
                        int j = jm[k];
                        double dist = PointD.Distance(xynew[i], xy[j]);
                        if(dist == 0)
                            dist = eps;
                        s += z[j] / dist;
                        w += 1.0 / dist;
                    }
                }

                if(i % 1000 == 0)
                    guider.Write(".");

                znew[i] = s / w;
            }

            return znew;
        }

        private const double eps = 1e-10;
    }
}
