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

            double minx = intervalx[0];
            double maxx = intervalx[1];
            double miny = intervaly[0];
            double maxy = intervaly[1];

            ArrayList ep = new ArrayList();
            this.Recursion(0, ep, m1, m2, minx, maxx, miny, maxy, guider);

            PointVector result = new PointVector(ep.Count);
            int i = 0;
            foreach(PointD p in ep)
                result[i++] = p;

            return result;
        }

        private void Recursion(int level, ArrayList ep, Matrix m1, Matrix m2, double minx, double maxx, double miny, double maxy, Guider guider) {
            PointVector ls = this.LevelSwap(m1, m2, minx, maxx, miny, maxy);

            if(guider != null)
                guider.Write("(" + level + "," + ls.Length + ")");

            if(ls.Length > 0) {
                if(level >= 5) {
                    PointD p = new PointD(0.5 * (minx + maxx), 0.5 * (miny + maxy));
                    ep.Add(p);
                    guider.Write(string.Format("[{0:0.000},{1:0.000}]", p.X, p.Y));
                }
                else {
                    level++;
                    this.Recursion(level, ep, m1, m2, minx, minx + (maxx - minx) / 2, miny, miny + (maxy - miny) / 2, guider);
                    this.Recursion(level, ep, m1, m2, minx, minx + (maxx - minx) / 2, miny + (maxy - miny) / 2, maxy, guider);
                    this.Recursion(level, ep, m1, m2, minx + (maxx - minx) / 2, maxx, miny, miny + (maxy - miny) / 2, guider);
                    this.Recursion(level, ep, m1, m2, minx + (maxx - minx) / 2, maxx, miny + (maxy - miny) / 2, maxy, guider);
                }
            }
        }

        private PointVector LevelSwap(Matrix m1, Matrix m2, double minx, double maxx, double miny, double maxy) {

            int length = m1.Length;

            double stepx = (maxx - minx) / 100;
            double stepy = (maxy - miny) / 100;

            // Init
            PointVector last = new PointVector(length);     // Last set of eigenvalues
            PointVector first = new PointVector(length);     // First set of eigenvalues
            CMatrix c0 = new CMatrix(length);
            for(int j = 0; j < length; j++)
                for(int k = 0; k < length; k++) {
                    c0[j, 2 * k] = m1[j, k] + minx * m2[j, k];
                    c0[j, 2 * k + 1] = miny * m2[j, k];
                }
            Vector[] v0 = c0.EigenSystem(false, length, null);
            for(int i = 0; i < length; i++) {
                last[i] = new PointD(v0[0][i], v0[1][i]);
                first[i] = new PointD(v0[0][i], v0[1][i]);
            }

            double x = minx;
            double y = miny;

            double xold = x;
            double yold = y;

            while(xold < maxx) {
                x = System.Math.Min(xold + stepx, maxx);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepx *= 1.1;
                    xold = x;
                }
                else 
                    stepx /= 2.0;

                if(stepx == 0)
                    continue;
            }

            while(yold < maxy) {
                y = System.Math.Min(yold + stepy, maxy);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepy *= 1.1;
                    yold = y;
                }
                else 
                    stepy /= 2.0;

                if(stepy == 0)
                    continue;
            }
        
            while(xold > minx) {
                x = System.Math.Max(xold - stepx, minx);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepx *= 1.1;
                    xold = x;
                }
                else 
                    stepx /= 2.0;

                if(stepx == 0)
                    continue;
            }

            while(yold > miny) {
                y = System.Math.Max(yold - stepy, miny);

                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + x * m2[j, k];
                        c[j, 2 * k + 1] = y * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);

                if(this.Connect(last, v)) {
                    stepy *= 1.1;
                    yold = y;
                }
                else 
                    stepy /= 2.0;

                if(stepy == 0)
                    continue;
            }

            ArrayList a = new ArrayList();
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    if(first[i].X == last[j].X && first[i].Y == last[j].Y) {
                        if(i != j)
                            a.Add(new PointD(i, j));
                    }

            PointVector result = new PointVector(a.Count);
            for(int i = 0; i < a.Count; i++)
                result[i] = (PointD)a[i];

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
            double spacing = this.MinSpacing(v);

            bool[] ks = new bool[length];

            for(int j = 0; j < length; j++) {
                double distance = double.MaxValue;
                PointD point = new PointD();
                PointD lastPoint = last[j];

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

                if(distance > 0.5 * spacing) {
                    return false;
                }

                last[j] = point;
                ks[k0] = true;
            }            

            return true;
        }
    }
}
