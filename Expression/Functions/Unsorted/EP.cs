using System;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Expression;
using PavelStransky.Math;

using MPIR;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// EP of a matrix in the form A + l B
    /// </summary>
    public class EP : Fnc {
        public override string Help { get { return Messages.HelpResultant; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));

            this.SetParam(2, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m1 = arguments[0] as Matrix;
            Matrix m2 = arguments[1] as Matrix;

            Vector intervalx = arguments[2] as Vector;
            Vector intervaly = arguments[3] as Vector;

            int length = m1.Length;

            double minx = intervalx[0];
            double maxx = intervalx[1];
            int numx = (int)intervalx[2];
            double stepx = (maxx - minx) / numx;

            double miny = intervaly[0];
            double maxy = intervaly[1];
            int numy = (int)intervaly[2];
            double stepy = (maxy - miny) / numy;

            ArrayList[] result = new ArrayList[length];
            for(int i = 0; i < length; i++)
                result[i] = new ArrayList();

            double x = minx;
            double y = miny;

            double xold = x;
            while(x < maxx) {
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(result, v)) {
                    stepx *= 1.1;
                    xold = x;
                    x += stepx;
                }
                else {
                    stepx /= 2.0;
                    x = xold + stepx;
                }
            }

            x = maxx;
            double yold = y;
            while(y < maxy) {
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(result, v)) {
                    stepy *= 1.1;
                    yold = y;
                    y += stepy;
                }
                else {
                    stepy /= 2.0;
                    y = yold + stepy;
                }
            }

            y = maxy;
            xold = x;
            while(x > minx) {
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(result, v)) {
                    stepx *= 1.1;
                    xold = x;
                    x -= stepx;
                }
                else {
                    stepx /= 2.0;
                    x = xold - stepx;
                }
            }

            x = minx;
            yold = y;
            while(y > miny) {
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(result, v)) {
                    stepy *= 1.1;
                    yold = y;
                    y -= stepy;
                }
                else {
                    stepy /= 2.0;
                    y = yold - stepy;
                }
            }

            return new TArray(this.ToPointVectors(result));
        }

        private PointVector[] ToPointVectors(ArrayList[] a) {
            int length = a.Length;
            PointVector[] result = new PointVector[length];

            for(int i = 0; i < length; i++) {
                int c = a[i].Count;
                result[i] = new PointVector(c);
                int j = 0;
                foreach(PointD p in a[i])
                    result[i][j++] = p;
            }
            return result;
        }

        private double MinSpacing(Vector[] v) {
            int length = v[0].Length;

            double distance = double.MaxValue;
            for(int i = 0; i < length; i++) {
                PointD p = new PointD(v[0][i], v[1][i]);
                for(int j = i + 1; j < length; j++)
                    distance = System.Math.Min(distance, PointD.Distance(p, new PointD(v[0][j], v[1][j])));
            }

            return distance;
        }

        private bool Connect(ArrayList[] result, Vector[] v) {
            int length = result.Length;
            double spacing = this.MinSpacing(v);

            bool[] ks = new bool[length];

            for(int j = 0; j < length; j++) {

                if(result[j].Count == 0)
                    result[j].Add(new PointD(v[0][j], v[1][j]));
                else {
                    double distance = double.MaxValue;
                    PointD point = new PointD();
                    PointD lastPoint = (PointD)result[j][result[j].Count - 1];

                    int k0 = -1;

                    for(int k = 0; k < length; k++) {
                        PointD p = new PointD(v[0][k], v[1][k]);
                        double d = PointD.Distance(lastPoint, p);
                        if(d < distance && !ks[k]) {
                            point = p;
                            distance = d;
                            k0 = k;
                        }
                    }

                    if(distance > spacing) {
                        return false;
                    }
                    result[j].Add(point);
                    ks[k0] = true;
                }
            }

            return true;
        }
    }
}
