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
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));

            this.SetParam(3, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(4, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m1 = arguments[0] as Matrix;
            Matrix m2 = arguments[1] as Matrix;
            Matrix m3 = arguments[2] as Matrix;

            Vector intervalx = arguments[3] as Vector;
            Vector intervaly = arguments[4] as Vector;

            double minx = intervalx[0];
            double maxx = intervalx[1];
            double miny = intervaly[0];
            double maxy = intervaly[1];

            double precisionx = intervalx[2];
            double precisiony = intervaly[2];

            ArrayList ep = new ArrayList();
            this.Recursion(0, ep, m1, m2, m3, minx, maxx, miny, maxy, precisionx, precisiony, guider);

            PointVector result = new PointVector(ep.Count);
            int i = 0;
            foreach(PointD p in ep)
                result[i++] = p;

            return result;
        }

        private void Recursion(int level, ArrayList ep, Matrix m1, Matrix m2, Matrix m3, double minx, double maxx, double miny, double maxy, double precisionx, double precisiony, Guider guider) {
            PointVector ls = this.LevelSwap(m1, m2, m3, minx, maxx, miny, maxy);

            if(ls.Length > 0) {
                guider.Write(".");
                if(maxx - minx < precisionx && maxy - miny < precisiony) {
                    PointD p = new PointD(0.5 * (minx + maxx), 0.5 * (miny + maxy));
                    ep.Add(p);
                    guider.Write(level);
                    for(int i = 0; i < ls.Length; i++)
                        guider.Write(string.Format("({0:0}-{1:0})", ls[i].X, ls[i].Y));
                    guider.WriteLine(string.Format("[{0:0.000},{1:0.000}]", p.X, p.Y));
                }
                else {
                    level++;
                    this.Recursion(level, ep, m1, m2, m3, minx, minx + (maxx - minx) / 2, miny, miny + (maxy - miny) / 2, precisionx, precisiony, guider);
                    this.Recursion(level, ep, m1, m2, m3, minx, minx + (maxx - minx) / 2, miny + (maxy - miny) / 2, maxy, precisionx, precisiony, guider);
                    this.Recursion(level, ep, m1, m2, m3, minx + (maxx - minx) / 2, maxx, miny, miny + (maxy - miny) / 2, precisionx, precisiony, guider);
                    this.Recursion(level, ep, m1, m2, m3, minx + (maxx - minx) / 2, maxx, miny + (maxy - miny) / 2, maxy, precisionx, precisiony, guider);
                }
            }
        }

        private PointVector LevelSwap(Matrix m1, Matrix m2, Matrix m3, double minx, double maxx, double miny, double maxy) {
            int length = m1.Length;

            double stepx = (maxx - minx) / 100;
            double stepy = (maxy - miny) / 100;

            double x = minx;
            double y = miny;

            double xold = x;
            double yold = y;

            // Init
            PointVector last = new PointVector(length);     // Last set of eigenvalues
            PointVector first = new PointVector(length);     // First set of eigenvalues
            CMatrix c0 = new CMatrix(length);
            for(int j = 0; j < length; j++)
                for(int k = 0; k < length; k++) {
                    c0[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                    c0[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                }
            Vector[] v0 = c0.EigenSystem(false, length, null);
            for(int i = 0; i < length; i++) {
                last[i] = new PointD(v0[0][i], v0[1][i]);
                first[i] = new PointD(v0[0][i], v0[1][i]);
            }

            while(xold < maxx) {
                x = System.Math.Min(xold + stepx, maxx);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                        c[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepx *= 1.1;
                    xold = x;
                }
                else
                    stepx /= 2.0;
            }

            while(yold < maxy) {
                y = System.Math.Min(yold + stepy, maxy);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                        c[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepy *= 1.1;
                    yold = y;
                }
                else
                    stepy /= 2.0;
            }

            while(xold > minx) {
                x = System.Math.Max(xold - stepx, minx);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                        c[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepx *= 1.1;
                    xold = x;
                }
                else
                    stepx /= 2.0;
            }

            while(yold > miny) {
                y = System.Math.Max(yold - stepy, miny);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                        c[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepy *= 1.1;
                    yold = y;
                }
                else
                    stepy /= 2.0;
            }

            ArrayList a = new ArrayList();
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    if(first[i].X == last[j].X && first[i].Y == last[j].Y) {
                        if(i != j) {
                            a.Add(i);
                            a.Add(j);
                        }
                    }
/*
            while(yold > 0) {
                y = System.Math.Max(yold - stepy, 0);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                        c[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepy *= 1.1;
                    yold = y;
                }
                else
                    stepy /= 2.0;
            }
  */
            int[] sort = new int[length];
            double[] keys = new double[length];
            for(int i = 0; i < length; i++) {
                sort[i] = i;
                keys[i] = last[i].X;
            }

//            Array.Sort(keys, sort);

            PointVector result = new PointVector(a.Count / 2);
            for(int i = 0; i < a.Count; i += 2)
                result[i / 2] = new PointD(sort[(int)a[i]], sort[(int)a[i + 1]]);

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

        private bool Connect(PointVector last, Vector[] v) {
            int length = last.Length;

            double ms = this.MinSpacing(v);

            bool[] ks = new bool[length];
            Matrix ds = new Matrix(length);

            PointVector np = new PointVector(length);

            for(int j = 0; j < length; j++) {
                double distance = double.MaxValue;
                PointD point = new PointD();
                PointD lastPoint = last[j];

                int k0 = -1;

                for(int k = 0; k < length; k++) {
                    PointD p = new PointD(v[0][k], v[1][k]);
                    double d = PointD.Distance(lastPoint, p);
                    ds[j, k] = d;
                    if(d < distance) {
                        point = p;
                        distance = d;
                        k0 = k;
                    }
                }

                if(ks[k0] || distance >= ms)
                    return false;

                np[j] = point;
                ks[k0] = true;
            }

            for(int j = 0; j < length; j++)
                last[j] = np[j];

            return true;
        }
    }
}
