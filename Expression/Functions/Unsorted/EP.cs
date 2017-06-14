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
    /// EP of a matrix in the form A + l B (+ l^2 C)
    /// </summary>
    public class EP : Fnc {
        public override string Help { get { return Messages.HelpEP; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));

            this.SetParam(3, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(4, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m1 = arguments[0] as Matrix;     // Matrix A
            Matrix m2 = arguments[1] as Matrix;     // Matrix B
            Matrix m3 = arguments[2] as Matrix;     // Matrix C (optional)

            Vector intervalx = arguments[3] as Vector;  // Interval x (minx, maxx, precisionx)
            Vector intervaly = arguments[4] as Vector;  // Interval y (miny, maxy, precisiony)

            double minx = intervalx[0];
            double maxx = intervalx[1];
            double miny = intervaly[0];
            double maxy = intervaly[1];

            double precisionx = intervalx[2];
            double precisiony = intervaly[2];

            if(guider != null) {
                guider.Write("Expected maximum number of EPs: ");
                if(m3 == null)
                    guider.WriteLine(m1.Length * (m1.Length - 1));
                else
                    guider.WriteLine(2 * m1.Length * (m1.Length - 1));
            }

            // Final EPs
            ArrayList ep = new ArrayList();
            this.Recursion(0, ep, m1, m2, m3, minx, maxx, miny, maxy, precisionx, precisiony, guider);

            // Finding levels that cross (Algorithm by M. Dvořák)
            int count = ep.Count;

            PointVector points = new PointVector(count);
            PointVector crossings = new PointVector(count);
            Vector keys = new Vector(count);

            int i = 0;
            foreach(PointD p in ep) {
                points[i] = p;
                crossings[i] = this.FindLevels(m1, m2, m3, p.X, p.Y);
                keys[i] = crossings[i].X * count + crossings[i].Y;
                i++;
            }

            points = points.Sort(keys) as PointVector;
            crossings = crossings.Sort(keys) as PointVector;

            if(guider != null)
            for(i = 0; i < count; i++)
                guider.WriteLine(string.Format("{0}[{1:0.00000},{2:0.00000}] ({3}-{4})", i, points[i].X, points[i].Y, crossings[i].X, crossings[i].Y));                        

            List result = new List();
            result.Add(points);
            result.Add(crossings);

            return result;
        }

        /// <summary>
        /// Recursive procedure
        /// </summary>
        /// <param name="level">Level of the recursion</param>
        /// <param name="ep">EPs (result)</param>
        /// <param name="m1">Matrix A</param>
        /// <param name="m2">Matrix B</param>
        /// <param name="m3">Matrix C</param>
        /// <param name="minx">Minimum x</param>
        /// <param name="maxx">Maximum x</param>
        /// <param name="miny">Minimum y</param>
        /// <param name="maxy">Maximum y</param>
        /// <param name="precisionx">Precision x</param>
        /// <param name="precisiony">Precision y</param>
        private void Recursion(int level, ArrayList ep, Matrix m1, Matrix m2, Matrix m3, double minx, double maxx, double miny, double maxy, double precisionx, double precisiony, Guider guider) {
            PointVector ls = this.LevelSwap(m1, m2, m3, minx, maxx, miny, maxy);

            // Maximum number of iteration reached
            if(ls == null) {
                if(guider != null)
                    guider.WriteLine("X");
            }
            // We found at least one level crossing (Be aware that for systems with m3 != null the level crossings can destroy each other!)
            else if(ls.Length > 0) {
                // Desired precision reached
                if(maxx - minx < precisionx && maxy - miny < precisiony) {

                    PointD p = new PointD(0.5 * (minx + maxx), 0.5 * (miny + maxy));    // The best approximation of an ep
                    ep.Add(p);

                    // Guider info
                    if(guider != null) {
                        guider.Write(string.Format("{0}-{1}", ep.Count, level));
                        for(int i = 0; i < ls.Length; i++)
                            guider.Write(string.Format("({0:0}-{1:0})", ls[i].X, ls[i].Y));
                        guider.WriteLine(string.Format("[{0:0.00000},{1:0.00000}]", p.X, p.Y));
                    }
                }
                else {
                    level++;
                    if(maxx - minx < precisionx) {
                        this.Recursion(level, ep, m1, m2, m3, minx, maxx, miny, miny + (maxy - miny) / 2, precisionx, precisiony, guider);
                        this.Recursion(level, ep, m1, m2, m3, minx, maxx, miny + (maxy - miny) / 2, maxy, precisionx, precisiony, guider);
                    }
                    else if(maxy - miny < precisiony) {
                        this.Recursion(level, ep, m1, m2, m3, minx, minx + (maxx - minx) / 2, miny, maxy, precisionx, precisiony, guider);
                        this.Recursion(level, ep, m1, m2, m3, minx + (maxx - minx) / 2, maxx, miny, maxy, precisionx, precisiony, guider);
                    }
                    else {
                        this.Recursion(level, ep, m1, m2, m3, minx, minx + (maxx - minx) / 2, miny, miny + (maxy - miny) / 2, precisionx, precisiony, guider);
                        this.Recursion(level, ep, m1, m2, m3, minx, minx + (maxx - minx) / 2, miny + (maxy - miny) / 2, maxy, precisionx, precisiony, guider);
                        this.Recursion(level, ep, m1, m2, m3, minx + (maxx - minx) / 2, maxx, miny, miny + (maxy - miny) / 2, precisionx, precisiony, guider);
                        this.Recursion(level, ep, m1, m2, m3, minx + (maxx - minx) / 2, maxx, miny + (maxy - miny) / 2, maxy, precisionx, precisiony, guider);
                    }
                }
            }
        }

        /// <summary>
        /// Calculation of levels that swap when we go around an exceptional point
        /// </summary>
        /// <param name="m1">matrix A</param>
        /// <param name="m2">matrix B</param>
        /// <param name="m3">matrix C</param>
        /// <param name="minx">Minimum x</param>
        /// <param name="maxx">Maximum x</param>
        /// <param name="miny">Minimum y</param>
        /// <param name="maxy">Maximum y</param>
        private PointVector LevelSwap(Matrix m1, Matrix m2, Matrix m3, double minx, double maxx, double miny, double maxy) {
            int length = m1.Length;

            double stepx = (maxx - minx) / initStep;
            double stepy = (maxy - miny) / initStep;

            double x = minx;
            double y = miny;

            double oldx = x;        // Temporary values (for the adaptive step)
            double oldy = y;

            // Initialization
            PointVector last = new PointVector(length);     // Last set of eigenvalues
            PointVector first = new PointVector(length);     // First set of eigenvalues
            Vector[] v0 = this.EigenSystem(m1, m2, m3, x, y);
            for(int i = 0; i < length; i++) {
                last[i] = new PointD(v0[0][i], v0[1][i]);
                first[i] = new PointD(v0[0][i], v0[1][i]);
            }

            int steps = 0;
            // Leg 1
            while(oldx < maxx) {
                x = System.Math.Min(oldx + stepx, maxx);
                if(this.Connect(last, this.EigenSystem(m1, m2, m3, x, y))) {
                    stepx *= stepM;
                    oldx = x;
                }
                else if((stepx /= stepD) == 0 || steps > maxSteps)
                        return null;
                steps++;
            }            

            // Leg 2
            while(oldy < maxy) {
                y = System.Math.Min(oldy + stepy, maxy);
                if(this.Connect(last, this.EigenSystem(m1, m2, m3, x, y))) {
                    stepy *= stepM;
                    oldy = y;
                }
                else
                    if((stepy /= stepD) == 0 || steps > maxSteps)
                        return null;
                steps++;
            }

            // Leg 3
            while(oldx > minx) {
                x = System.Math.Max(oldx - stepx, minx);
                if(this.Connect(last, this.EigenSystem(m1, m2, m3, x, y))) {
                    stepx *= stepM;
                    oldx = x;
                }
                else
                    if((stepx /= stepD) == 0 || steps > maxSteps)
                        return null;
                steps++;
            }

            // Leg 4
            while(oldy > miny) {
                y = System.Math.Max(oldy - stepy, miny);
                if(this.Connect(last, this.EigenSystem(m1, m2, m3, x, y))) {
                    stepy *= stepM;
                    oldy = y;
                }
                else {
                    if((stepy /= stepD) == 0 || steps > maxSteps)
                        return null;
                }
                steps++;
            }

            ArrayList a = new ArrayList();
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    if(first[i].X == last[j].X && first[i].Y == last[j].Y) 
                        if(i != j) 
                            a.Add(new PointD(i,j));                                            

            PointVector result = new PointVector(a.Count);
            for(int i = 0; i < a.Count; i++)
                result[i] = (PointD)a[i];

            return result;
        }

        /// <summary>
        /// Calculates eigenvalues
        /// </summary>
        /// <param name="m1">Matrix A</param>
        /// <param name="m2">Matrix B</param>
        /// <param name="m3">Matrix C</param>
        /// <param name="x">Real value of the control parameter</param>
        /// <param name="y">Imaginary value of the control parameter</param>
        private Vector[] EigenSystem(Matrix m1, Matrix m2, Matrix m3, double x, double y) {
            int length = m1.Length;

            // Fills the matrix
            CMatrix c = new CMatrix(length);
            for(int j = 0; j < length; j++)
                for(int k = 0; k < length; k++) {
                    c[j, 2 * k] = m1[j, k] + x * m2[j, k] + (m3 != null ? (x * x - y * y) * m3[j, k] : 0.0);
                    c[j, 2 * k + 1] = y * m2[j, k] + (m3 != null ? 2.0 * x * y * m3[j, k] : 0.0);
                }

            // Calculates the eigenvalues
            return c.EigenSystem(false, length, null);
        }

        /// <summary>
        /// Calculates the minimum spacing between complex eigenvalues
        /// </summary>
        /// <param name="v">Eigenvalues (Real and Imaginary part)</param>
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

        /// <summary>
        /// Tries to connect eigenvalues to the last values
        /// </summary>
        /// <param name="last">Last eigenvalues</param>
        /// <param name="v">New eigenvalues</param>
        private bool Connect(PointVector last, Vector[] v) {
            int length = last.Length;

            double ms = this.MinSpacing(v);

            // Already used new eigenvalues
            bool[] ks = new bool[length];

            // New eigenvalues
            PointVector np = new PointVector(length);

            for(int j = 0; j < length; j++) {
                double distance = double.MaxValue;
                PointD point = new PointD();
                PointD lastPoint = last[j];

                int k0 = -1;

                for(int k = 0; k < length; k++) {
                    PointD p = new PointD(v[0][k], v[1][k]);
                    double d = PointD.Distance(lastPoint, p);
                    if(d < distance) {
                        point = p;
                        distance = d;
                        k0 = k;
                    }
                }

                // We were not able to find a unique connection - the step must be reduced
                if(ks[k0] || distance >= ms)
                    return false;

                np[j] = point;
                ks[k0] = true;
            }

            // Copy of new points
            for(int j = 0; j < length; j++)
                last[j] = np[j];

            return true;
        }

        private PointD FindLevels(Matrix m1, Matrix m2, Matrix m3, double x, double maxy) {
            int length = m1.Length;
            
            double y = 0.0;
            double oldy = 0.0;
            double stepy = maxy / 100;

            // Initialization
            PointVector last = new PointVector(length);     // Last set of eigenvalues
            PointVector first = new PointVector(length);     // First set of eigenvalues
            Vector[] v0 = this.EigenSystem(m1, m2, m3, x, y);
            for(int i = 0; i < length; i++) {
                last[i] = new PointD(v0[0][i], v0[1][i]);
                first[i] = new PointD(v0[0][i], v0[1][i]);
            }
            last = last.Sort() as PointVector;
            first = first.Sort() as PointVector;

            // Leg 2
            while(oldy < maxy) {
                y = System.Math.Min(oldy + stepy, maxy);
                if(this.Connect(last, this.EigenSystem(m1, m2, m3, x, y))) {
                    stepy *= stepM;
                    oldy = y;
                }
                else
                    if((stepy /= stepD) == 0)
                        throw new Exception("StepY has reched value 0");
            }

            // Distance matrix
            Matrix distance = new Matrix(length);
            distance.Fill(double.MaxValue);

            for(int i = 0; i < length; i++)
                for(int j = i + 1; j < length; j++)
                    distance[i, j] = PointD.Distance(last[i], last[j]);

            int[] mindistance = distance.MinIndex();

            return new PointD(mindistance[0], mindistance[1]);
        }

        private const double stepM = 1.2;
        private const double stepD = 2;
        private const double maxSteps = 100000;
        private const double initStep = 1000;
    }
}
