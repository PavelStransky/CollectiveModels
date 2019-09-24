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
    /// EP of a matrix in the form A + l B (+ l^2 C) using various methods
    /// </summary>
    public class EP : Fnc {
        public override string Help { get { return Messages.HelpEP; } }

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));

            this.SetParam(3, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(4, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));

            this.SetParam(5, false, true, false, Messages.PType, Messages.PTypeDescription, 0, typeof(int));
        }

        /// <summary>
        /// Class taking all the data
        /// </summary>
        private class Data {
            public Matrix m1, m2, m3;
            public double precisionx, precisiony;
            public int steps = 0;
            public Guider guider;
            public int length;

            public DateTime time;

            public Data(Matrix m1, Matrix m2, Matrix m3, double precisionx, double precisiony, Guider guider) {
                this.m1 = m1;
                this.m2 = m2;
                this.m3 = m3;
                this.precisionx = precisionx;
                this.precisiony = precisiony;
                this.guider = guider;
                this.length = m1.Length;

                this.time = DateTime.Now;
            }

            public Matrix M1 { get { return m1; } }
            public Matrix M2 { get { return m2; } }
            public Matrix M3 { get { return m3; } }
            public double Precisionx { get { return precisionx; } }
            public double Precisiony { get { return precisiony; } }
            public int Length { get { return this.length; } }
            public int Steps { get { return this.steps; } }

            /// <summary>
            /// Add steps to the total steps counter
            /// </summary>
            /// <param name="steps">Number of steps to add</param>
            public void AddSteps(int steps) {
                this.steps += steps;
            }

            /// <summary>
            /// Write text to the Writer
            /// </summary>
            /// <param name="s">Text</param>
            public void Write(string s) {
                if(this.guider != null)
                    this.guider.Write(s);
            }

            /// <summary>
            /// Write text to the Writer and end line
            /// </summary>
            /// <param name="s">Text</param>
            public void WriteLine(string s) {
                if(this.guider != null)
                    this.guider.WriteLine(s);
            }

            public void WriteTime() {
                this.WriteLine(SpecialFormat.Format(DateTime.Now - this.time));
                this.time = DateTime.Now;
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m1 = arguments[0] as Matrix;     // Matrix A
            Matrix m2 = arguments[1] as Matrix;     // Matrix B
            Matrix m3 = arguments[2] as Matrix;     // Matrix C (optional)

            Vector intervalx = arguments[3] as Vector;  // Interval x (minx, maxx, precisionx)
            Vector intervaly = arguments[4] as Vector;  // Interval y (miny, maxy, precisiony)

            int type = (int)arguments[5];           // Type of the calculation (0 - level swap, 1 - Zirnbauer's phase)

            double minx = intervalx[0];
            double maxx = intervalx[1];
            double miny = intervaly[0];
            double maxy = intervaly[1];

            double precisionx = intervalx[2];
            double precisiony = intervaly[2];

            // Initiate data class
            Data data = new Data(m1, m2, m3, precisionx, precisiony, guider);

            data.Write("Maximum number of EPs: ");
            if(m3 == null)
                data.WriteLine((m1.Length * (m1.Length - 1)).ToString());
            else
                data.WriteLine((2 * m1.Length * (m1.Length - 1)).ToString());

            // Final EPs
            ArrayList ep = new ArrayList();

            // Main recursion calculation
            if(type == 0)
                this.RecursionSwap(0, ep, minx, maxx, miny, maxy, data);
            else
                this.RecursionPhase(0, ep, minx, maxx, miny, maxy, data);

            // Finding levels that cross (Algorithm by M. Dvořák)
            int count = ep.Count;

            PointVector points = new PointVector(count);
            PointVector crossings = new PointVector(count);
            PointVector energies = new PointVector(count);
            Vector keys = new Vector(count);

            int i = 0;
            foreach(PointD p in ep) {
                points[i] = p;
                ArrayList cr = this.FindLevels(p.X, p.Y, data);
                crossings[i] = (PointD)cr[0];
                energies[i] = (PointD)cr[1];
                keys[i] = crossings[i].X * count + crossings[i].Y;
                i++;
            }

            points = points.Sort(keys) as PointVector;
            crossings = crossings.Sort(keys) as PointVector;
            energies = energies.Sort(keys) as PointVector;

            if(guider != null)
                for(i = 0; i < count; i++)
                    guider.WriteLine(string.Format("{0}[{1},{2}] ({3}-{4})", i, points[i].X, points[i].Y, crossings[i].X, crossings[i].Y));

            List result = new List();
            result.Add(points);
            result.Add(crossings);
            result.Add(energies);

            return result;
        }

        #region Level Swap
        /// <summary>
        /// Recursive procedure
        /// </summary>
        /// <param name="level">Level of the recursion</param>
        /// <param name="ep">EPs (result)</param>
        /// <param name="minx">Minimum x</param>
        /// <param name="maxx">Maximum x</param>
        /// <param name="miny">Minimum y</param>
        /// <param name="maxy">Maximum y</param>
        private void RecursionSwap(int level, ArrayList ep, double minx, double maxx, double miny, double maxy, Data data) {
            PointVector ls = this.LevelSwap(minx, maxx, miny, maxy, data);

            // Maximum number of iteration reached
            if(ls == null) {
                data.WriteLine("X");
            }
            // We found at least one level crossing (Be aware that for systems with m3 != null the level crossings can destroy each other!)
            else if(ls.Length > 0) {
                if(level == 0)
                    data.WriteLine(string.Format("Expected number of EPs in the interval: {0}", ls.Length));

                // Desired precision reached
                if(maxx - minx < data.Precisionx && maxy - miny < data.Precisiony) {

                    PointD p = new PointD(0.5 * (minx + maxx), 0.5 * (miny + maxy));    // The best approximation of an ep
                    ep.Add(p);

                    // Guider info
                    data.Write(string.Format("{0}-{1}", ep.Count, level));
                    for(int i = 0; i < ls.Length; i++)
                        data.Write(string.Format("({0:0}-{1:0}){2}", ls[i].X, ls[i].Y, data.Steps));
                    data.Write(string.Format("[{0},{1}]", p.X, p.Y));
                    data.WriteTime();
                }
                else {
                    level++;
                    if(maxx - minx < data.Precisionx) {
                        this.RecursionSwap(level, ep, minx, maxx, miny, miny + (maxy - miny) / 2, data);
                        this.RecursionSwap(level, ep, minx, maxx, miny + (maxy - miny) / 2, maxy, data);
                    }
                    else if(maxy - miny < data.Precisiony) {
                        this.RecursionSwap(level, ep, minx, minx + (maxx - minx) / 2, miny, maxy, data);
                        this.RecursionSwap(level, ep, minx + (maxx - minx) / 2, maxx, miny, maxy, data);
                    }
                    else {
                        this.RecursionSwap(level, ep, minx, minx + (maxx - minx) / 2, miny, miny + (maxy - miny) / 2, data);
                        this.RecursionSwap(level, ep, minx, minx + (maxx - minx) / 2, miny + (maxy - miny) / 2, maxy, data);
                        this.RecursionSwap(level, ep, minx + (maxx - minx) / 2, maxx, miny, miny + (maxy - miny) / 2, data);
                        this.RecursionSwap(level, ep, minx + (maxx - minx) / 2, maxx, miny + (maxy - miny) / 2, maxy, data);
                    }
                }
            }
        }

        /// <summary>
        /// Calculation of levels that swap when we go around an exceptional point
        /// </summary>
        /// <param name="minx">Minimum x</param>
        /// <param name="maxx">Maximum x</param>
        /// <param name="miny">Minimum y</param>
        /// <param name="maxy">Maximum y</param>
        private PointVector LevelSwap(double minx, double maxx, double miny, double maxy, Data data) {
            int length = data.Length;

            double stepx = (maxx - minx) / initStep;
            double stepy = (maxy - miny) / initStep;

            double x = minx;
            double y = miny;

            double oldx = x;        // Temporary values (for the adaptive step)
            double oldy = y;

            // Initialization
            PointVector last = new PointVector(length);     // Last set of eigenvalues
            PointVector first = new PointVector(length);     // First set of eigenvalues
            Vector[] v0 = this.EigenSystem(x, y, data);
            for(int i = 0; i < length; i++) {
                last[i] = new PointD(v0[0][i], v0[1][i]);
                first[i] = new PointD(v0[0][i], v0[1][i]);
            }

            int steps = 0;

            // Leg 1
            while(oldx < maxx) {
                x = System.Math.Min(oldx + stepx, maxx);
                if(this.Connect(last, this.EigenSystem(x, y, data))) {
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
                if(this.Connect(last, this.EigenSystem(x, y, data))) {
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
                if(this.Connect(last, this.EigenSystem(x, y, data))) {
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
                if(this.Connect(last, this.EigenSystem(x, y, data))) {
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

            data.AddSteps(steps);
            return result;
        }
        #endregion

        #region Phase
        /// <summary>
        /// Recursive procedure
        /// </summary>
        /// <param name="level">Level of the recursion</param>
        /// <param name="ep">EPs (result)</param>
        /// <param name="minx">Minimum x</param>
        /// <param name="maxx">Maximum x</param>
        /// <param name="miny">Minimum y</param>
        /// <param name="maxy">Maximum y</param>
        private void RecursionPhase(int level, ArrayList ep, double minx, double maxx, double miny, double maxy, Data data) {
            double num = this.Phase(minx, maxx, miny, maxy, data);

            // Maximum number of iteration reached
            if(num < 0) {
                data.WriteLine("X");
            }
            // We found at least one level crossing (Be aware that for systems with m3 != null the level crossings can destroy each other!)
            else if(System.Math.Round(num) > 0) {
                if(level == 0)
                    data.WriteLine(string.Format("Expected number of EPs in the interval: {0}", System.Math.Round(num)));

                // Desired precision reached
                if(maxx - minx < data.Precisionx && maxy - miny < data.Precisiony) {

                    PointD p = new PointD(0.5 * (minx + maxx), 0.5 * (miny + maxy));    // The best approximation of an ep
                    ep.Add(p);

                    // Guider info
                    data.Write(string.Format("{0}-{1}({2}){3}", ep.Count, level, num, data.Steps));
                    data.Write(string.Format("[{0},{1}]", p.X, p.Y));
                    data.WriteTime();
                }
                else {
                    level++;
                    if(maxx - minx < data.Precisionx) {
                        this.RecursionPhase(level, ep, minx, maxx, miny, miny + (maxy - miny) / 2, data);
                        this.RecursionPhase(level, ep, minx, maxx, miny + (maxy - miny) / 2, maxy, data);
                    }
                    else if(maxy - miny < data.Precisiony) {
                        this.RecursionPhase(level, ep, minx, minx + (maxx - minx) / 2, miny, maxy, data);
                        this.RecursionPhase(level, ep, minx + (maxx - minx) / 2, maxx, miny, maxy, data);
                    }
                    else {
                        this.RecursionPhase(level, ep, minx, minx + (maxx - minx) / 2, miny, miny + (maxy - miny) / 2, data);
                        this.RecursionPhase(level, ep, minx, minx + (maxx - minx) / 2, miny + (maxy - miny) / 2, maxy, data);
                        this.RecursionPhase(level, ep, minx + (maxx - minx) / 2, maxx, miny, miny + (maxy - miny) / 2, data);
                        this.RecursionPhase(level, ep, minx + (maxx - minx) / 2, maxx, miny + (maxy - miny) / 2, maxy, data);
                    }
                }
            }
        }

        /// <summary>
        /// Calculation of phase when we go around exceptional points
        /// </summary>
        /// <param name="minx">Minimum x</param>
        /// <param name="maxx">Maximum x</param>
        /// <param name="miny">Minimum y</param>
        /// <param name="maxy">Maximum y</param>
        private double Phase(double minx, double maxx, double miny, double maxy, Data data) {
            int length = data.Length;

            double stepx = (maxx - minx) / initStep;
            double stepy = (maxy - miny) / initStep;

            double x = minx;
            double y = miny;

            double oldx = x;        // Temporary values (for the adaptive step)
            double oldy = y;

            // Initialization
            Vector[] v0 = this.EigenSystem(x, y, data);
            double last = this.ResultantPhase(v0);
            double phase = 0.0;

            int steps = 0;

            // Leg 1
            while(oldx < maxx) {
                x = System.Math.Min(oldx + stepx, maxx);
                double p = this.ResultantPhase(this.EigenSystem(x, y, data));
                double d = this.NormPhase(p - last);
                if(System.Math.Abs(d) < maxPhase) {
                    phase += d;
                    last = p;
                    stepx *= stepM;
                    oldx = x;
                }
                else if((stepx /= stepD) == 0 || steps > maxSteps)
                    return -1;
                steps++;
            }

            // Leg 2
            while(oldy < maxy) {
                y = System.Math.Min(oldy + stepy, maxy);
                double p = this.ResultantPhase(this.EigenSystem(x, y, data));
                double d = this.NormPhase(p - last);
                if(System.Math.Abs(d) < maxPhase) {
                    phase += d;
                    last = p;
                    stepy *= stepM;
                    oldy = y;
                }
                else
                    if((stepy /= stepD) == 0 || steps > maxSteps)
                        return -1;
                steps++;
            }

            // Leg 3
            while(oldx > minx) {
                x = System.Math.Max(oldx - stepx, minx);
                double p = this.ResultantPhase(this.EigenSystem(x, y, data));
                double d = this.NormPhase(p - last);
                if(System.Math.Abs(d) < maxPhase) {
                    phase += d;
                    last = p;
                    stepx *= stepM;
                    oldx = x;
                }
                else
                    if((stepx /= stepD) == 0 || steps > maxSteps)
                        return -1;
                steps++;
            }

            // Leg 4
            while(oldy > miny) {
                y = System.Math.Max(oldy - stepy, miny);
                double p = this.ResultantPhase(this.EigenSystem(x, y, data));
                double d = this.NormPhase(p - last);
                if(System.Math.Abs(d) < maxPhase) {
                    phase += d;
                    last = p;
                    stepy *= stepM;
                    oldy = y;
                }
                else {
                    if((stepy /= stepD) == 0 || steps > maxSteps)
                        return -1;
                }
                steps++;
            }

            data.AddSteps(steps);
            return System.Math.Abs(phase / (2 * System.Math.PI));
        }

        private double ResultantPhase(Vector[] v) {
            Vector re = v[0];
            Vector im = v[1];

            int length = re.Length;

            double result = 0;
            for(int i = 0; i < length; i++) {
                for(int j = 0; j < length; j++) {
                    if(i == j)
                        continue;
                    result += System.Math.Atan2(im[i] - im[j], re[i] - re[j]);
                }
            }

            return result;
        }

        private double NormPhase(double phase) {
            phase %= 2 * System.Math.PI;
            if(phase < -System.Math.PI)
                phase += 2 * System.Math.PI;
            else if(phase > System.Math.PI)
                phase = 2 * System.Math.PI - phase;
            return phase;
        }
        #endregion

        /// <summary>
        /// Calculates eigenvalues
        /// </summary>
        /// <param name="x">Real value of the control parameter</param>
        /// <param name="y">Imaginary value of the control parameter</param>
        private Vector[] EigenSystem(double x, double y, Data data) {
            int length = data.Length;

            // Fills the matrix
            CMatrix c = new CMatrix(length);
            for(int j = 0; j < length; j++)
                for(int k = 0; k < length; k++) {
                    c[j, 2 * k] = data.M1[j, k] + x * data.M2[j, k] + (data.M3 != null ? (x * x - y * y) * data.M3[j, k] : 0.0);
                    c[j, 2 * k + 1] = y * data.M2[j, k] + (data.M3 != null ? 2.0 * x * y * data.M3[j, k] : 0.0);
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

        private ArrayList FindLevels(double x, double maxy, Data data) {
            int length = data.Length;
            
            double y = 0.0;
            double oldy = 0.0;
            double stepy = maxy / 100;

            // Initialization
            PointVector last = new PointVector(length);     // Last set of eigenvalues
            PointVector first = new PointVector(length);     // First set of eigenvalues
            Vector[] v0 = this.EigenSystem(x, y, data);
            for(int i = 0; i < length; i++) {
                last[i] = new PointD(v0[0][i], v0[1][i]);
                first[i] = new PointD(v0[0][i], v0[1][i]);
            }
            last = last.Sort() as PointVector;
            first = first.Sort() as PointVector;

            // Leg 2
            while(oldy < maxy) {
                y = System.Math.Min(oldy + stepy, maxy);
                if(this.Connect(last, this.EigenSystem(x, y, data))) {
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

            int m1 = mindistance[0];
            int m2 = mindistance[1];

            ArrayList result = new ArrayList();
            result.Add(new PointD(m1, m2));
            result.Add(new PointD(0.5 * (last[m1].X + last[m2].X), 0.5 * (last[m1].Y + last[m2].Y)));
            return result;
        }

        private const double stepM = 1.2;
        private const double stepD = 2;
        private const double maxSteps = 100000;
        private const double initStep = 1000;
        private const double maxPhase = System.Math.PI / 2;
    }
}
