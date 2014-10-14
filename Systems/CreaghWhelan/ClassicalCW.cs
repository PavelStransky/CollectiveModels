using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class ClassicalCW : CW, IDynamicalSystem {
        private Random random = new Random();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="a">Parameter a (governs the PT)</param>
        /// <param name="b">Parameter b (rigidity)</param>
        /// <param name="c">Parameter c</param>
        /// <param name="mu">Parameter mu</param>
        /// <param name="power">Power of the principal term</param>
        public ClassicalCW(double a, double b, double c, double mu, int power) : base(a, b, c, mu, power) { }

        #region IDynamicalSystem Members
        public double T(double px, double py) {
            return 0.5 * (px * px + py * py);
        }

        public double E(Vector x) {
            return this.T(x[2], x[3]) + this.V(x[0], x[1]);
        }

        public Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4);

            double dV2dxdx = 12.0 * x[0] * x[0] + 2.0 * this.C * x[1] * x[1] - 4;
            double dV2dxdy = 2.0 * x[1] * (this.B + 2.0 * this.C * x[0]);
            double dV2dydy = 2.0 * (x[0] * (this.B + this.C * x[0]) + this.Mu);

            result[0, 2] = 1;
            result[1, 3] = result[0, 2];

            result[2, 0] = -dV2dxdx;
            result[2, 1] = -dV2dxdy;

            result[3, 0] = result[2, 1];
            result[3, 1] = -dV2dydy;

            return result;
        }

        public Vector Equation(Vector x) {
            Vector result = new Vector(4);

            result[0] = x[2];
            result[1] = x[3];

            double bracket = x[0] * x[0] - 1.0;

            if(this.Power == 2)
                result[2] = -this.A - 4.0 * x[0] * bracket - (this.B + 2.0 * this.C * x[0]) * x[1] * x[1];
            else if(this.Power == 4)
                result[2] = -this.A - 8.0 * x[0] * bracket * bracket * bracket - (this.B + 2.0 * this.C * x[0]) * x[1] * x[1];
            else if(this.Power == -4)
                result[2] = -this.A - 4.0 * x[0] * bracket - 8.0 * x[0] * bracket * bracket * bracket - (this.B + 2.0 * this.C * x[0]) * x[1] * x[1];

            result[3] = -2.0 * x[1] * (this.Mu + this.B * x[0] + this.C * x[0] * x[0]);

            return result;
        }

        public Vector IC(double e) {
            BisectionPotential bp = new BisectionPotential(this, 0.0, 0.0, e);
            Bisection b = new Bisection(bp.BisectionX);

            Vector result = new Vector(4);

            Vector roots = this.RootsX(e);
            if(roots.Length == 2)
                result[0] = random.NextDouble() * (roots[1] - roots[0]) +roots[0];
            else if(roots.Length == 3)
                result[0] = random.NextDouble() *(roots[2]-roots[0])+roots[0];
            else if(roots.Length == 4) {
                if(random.Next(2) == 0)
                    result[0] = random.NextDouble() * (roots[1] - roots[0]) + roots[0];
                else
                    result[0] = random.NextDouble() * (roots[3] - roots[2]) + roots[2];
            }

            // y
            bp = new BisectionPotential(this, result[0], 0.0, e);
            b = new Bisection(bp.BisectionY);

            double y = b.Solve(0, 10 + e + System.Math.Abs(this.B));
            result[1] = (2.0 * random.NextDouble() - 1.0) * y;

            result[2] = (2.0 * random.NextDouble() - 1.0) * System.Math.Sqrt(2.0 * (e - this.V(result[0], result[1])));
            result[3] = System.Math.Sqrt(2.0 * (e - this.V(result[0], result[1]) - this.T(result[2], 0.0)));
            if(random.Next(2) == 0)
                result[3] = -result[3];

            return result;
        }

        public Vector IC(double e, double l) {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IC(Vector ic, double e) {
            double tbracket = 2.0 * (e - this.V(ic[0], ic[1]));

            if(double.IsNaN(ic[2]) && double.IsNaN(ic[3])) {
                ic[2] = this.random.NextDouble() * System.Math.Sqrt(tbracket);

                if(this.random.Next(2) == 0)
                    ic[2] = -ic[2];
            }

            if(double.IsNaN(ic[2]))
                tbracket -= ic[3] * ic[3];
            else
                tbracket -= ic[2] * ic[2];

            if(tbracket < 0.0)
                return false;

            if(double.IsNaN(ic[2]))
                ic[2] = System.Math.Sqrt(tbracket) * (this.random.Next(2) == 0 ? -1.0 : 1.0);
            else
                ic[3] = System.Math.Sqrt(tbracket) * (this.random.Next(2) == 0 ? -1.0 : 1.0);

            return true;
        }

        private double VX(double x) {
            return this.A + 4.0 * x * (x * x - 1);
        }

        /// <summary>
        /// Minimum energy
        /// </summary>
        public double MinE() {
            // Minima
            Vector ext = this.ExtreemesX();
            Vector vext = new Vector(ext.Length);
            for(int i = 0; i < ext.Length; i++)
                vext[i] = this.V((double)ext[i], 0.0);
            double min = (double)ext[vext.MinIndex()];
            return vext.Min();
        }

        /// <summary>
        /// Extreemes in the form (x, y)
        /// </summary>
        public PointVector Extreemes() {
            Vector v = this.ExtreemesX();
            PointVector result = new PointVector(v.Length);

            for(int i = 0; i < v.Length; i++)
                result[i] = new PointD(v[i], this.V(v[i], 0.0));

            return result;
        }

        private Vector ExtreemesX() {
            // Extreemes
            BisectionDxPotential bdxp = new BisectionDxPotential(this);
            Bisection b = new Bisection(bdxp.Bisection);

            ArrayList ext = new ArrayList();
            double coef = 1.0 / System.Math.Sqrt(3.0);

            double x = b.Solve(-10 - System.Math.Abs(this.A), -coef);
            if(!double.IsNaN(x))
                ext.Add(x);
            x = b.Solve(-coef, coef);
            if(!double.IsNaN(x))
                ext.Add(x);
            x = b.Solve(coef, 10 + System.Math.Abs(this.A));
            if(!double.IsNaN(x))
                ext.Add(x);

            Vector result = new Vector(ext.Count);
            for(int i = 0; i < ext.Count; i++)
                result[i] = (double)ext[i];
            return result;
        }

        private Vector RootsX(double e) {
            Vector ext = this.ExtreemesX();

            // Solutions
            BisectionPotential bp = new BisectionPotential(this, 0.0, 0.0, e);
            Bisection b = new Bisection(bp.BisectionX);

            ArrayList sol = new ArrayList();

            double x = b.Solve(-10 - System.Math.Abs(this.A), (double)ext[0]);
            if(!double.IsNaN(x))
                sol.Add(x);
            for(int i = 0; i < ext.Length - 1; i++) {
                x = b.Solve((double)ext[i], (double)ext[i + 1]);
                if(!double.IsNaN(x))
                    sol.Add(x);
            }
            x = b.Solve((double)ext[ext.Length - 1], 10 + System.Math.Abs(this.A));
            if(!double.IsNaN(x))
                sol.Add(x);

            Vector result = new Vector(sol.Count);
            for(int i = 0; i < sol.Count; i++)
                result[i] = (double)sol[i];

            return result;
        }

        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            Vector sol = this.RootsX(e);

            result[0] = sol.FirstItem;
            result[1] = sol.LastItem;

            double pisvejc = 1E-6;

            // Minima
            Vector ext = this.ExtreemesX();
            Vector vext = new Vector(ext.Length);
            for(int i = 0; i < ext.Length; i++)
                vext[i] = this.V((double)ext[i], 0.0);
            double min = (double)ext[vext.MinIndex()];
            double vmin = vext.Min();

            // y
            double minxy = min;
            double maxxy = minxy < 0 ? System.Math.Min((double)sol[1] - pisvejc, 0.0) : System.Math.Max((double)sol[sol.Length - 2] + pisvejc, 0.0);

            BisectionY by = new BisectionY(this, e);
            Bisection b = new Bisection(by.Bisection);
            double y1 = b.Minimum(System.Math.Min(minxy, maxxy), System.Math.Max(minxy, maxxy));

            double y2 = 0.0;
            if(minxy < 0.0 && result[1] > 0.0)
                y2 = b.Minimum(System.Math.Max(0.0, (double)sol[sol.Length - 2] + pisvejc), result[1]);
            if(minxy > 0.0 && result[0] < 0.0)
                y2 = b.Minimum((double)result[0], System.Math.Min(0.0, (double)sol[1] - pisvejc));
            if(double.IsNaN(y2) || double.IsInfinity(y2))
                y2 = 0.0;           

            result[2] = System.Math.Min(by.Bisection(y1), by.Bisection(y2));
            result[3] = -result[2];

            result[5] = System.Math.Sqrt(2.0 * (e - vmin));
            result[4] = -result[5];

            result[6] = result[4];
            result[7] = result[5];

            return result;
        }

        private double BisectionCC(double e) {
            if(this.IsConvex(e, 1000))
                return 1.0;
            else
                return -1.0;
        }

        /// <summary>
        /// Convex-concave transition between the minimum and the given maximum energy
        /// </summary>
        /// <param name="precision">Precision of the step</param>
        public Vector ConvexConcave(double maxE, double precision) {
            double minE = this.MinE();

            double step = precision * (maxE - minE);

            double e = minE + step;
            double c = this.BisectionCC(e);

            Bisection b = new Bisection(this.BisectionCC);

            ArrayList r = new ArrayList();
            while(e < maxE) {
                double en = e + step;
                double cn = this.BisectionCC(en);

                if(c * cn < 0)
                    r.Add(b.Solve(e, en));

                e = en; c = cn;
            }

            Vector result = new Vector(r.Count);
            for(int i = 0; i < r.Count; i++) {
                result[i] = (double)r[i];
            }

            return result;
        }

        public Vector CheckBounds(Vector bounds) {
            return bounds;
        }

        public int DegreesOfFreedom {
            get { return 2; }
        }

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PostProcess(Vector x) {
            return false;
        }

        /// <summary>
        /// Body pro rozhodnutí, zda je podle SALI daná trajektorie regulární nebo chaotická
        /// </summary>
        /// <returns>[time chaotická, SALI chaotická, time regulární, SALI regulární, time koncový bod, SALI koncový bod]</returns>
        public double[] SALIDecisionPoints() {
            return new double[] { 0, 6, 500, 0, 1000, 10 };
        }
        #endregion

        public ClassicalCW(Core.Import import) : base(import) { }

        /// <summary>
        /// Equipotential contours
        /// </summary>
        /// <param name="e">Energy of the system</param>
        /// <param name="n">Number of points in each contour</param>
        public PointVector[] EquipotentialContours(double e, int n) {
            Vector sol = this.RootsX(e);

            PointVector[] result = new PointVector[0];

            if(sol.Length == 2 || sol.Length == 3) {
                result = new PointVector[1];
                result[0] = this.EquipotentialContour(e, n, sol.FirstItem, sol.LastItem);
            }
            else if(sol.Length == 4) {
                result = new PointVector[2];
                result[0] = this.EquipotentialContour(e, n, sol[0], sol[1]);
                result[1] = this.EquipotentialContour(e, n, sol[2], sol[3]);
            }

            return result;
        }

        /// <summary>
        /// One equipotential contour
        /// </summary>
        /// <param name="e">Energy of the system</param>
        /// <param name="n">Number of points in the contour</param>
        /// <param name="x1">Lower bound of the contour</param>
        /// <param name="x2">Upper bound of the contour</param>
        private PointVector EquipotentialContour(double e, int n, double x1, double x2) {
            PointVector contour = new PointVector(2 * n + 1);
            contour[0] = new PointD(x1, 0.0);
            for(int i = 1; i < n; i++) {
                double x = i * (x2 - x1) / n + x1;
                contour[i] = new PointD(x, System.Math.Sqrt((e - this.A * x - System.Math.Pow(x * x - 1.0, this.Power)) / (this.B * x + this.C * x * x + this.Mu)));
                contour[2 * n - i] = new PointD(x, -contour[i].Y);                
            }
            contour[n] = new PointD(x2, 0.0);
            contour[2 * n] = new PointD(x1, 0.0);
            return contour;
        }

        /// <summary>
        /// Napoèítá matici V
        /// (podle PRL 98, 234301 (2007))
        /// </summary>
        /// <param name="e">Energie</param>
        public Matrix VMatrix(double e, double x, double y) {
            double vx = 4.0 * x * (x * x - 1.0) + this.A + (this.B + 2.0 * this.C * x) * y * y;
            double vy = 2.0 * y * (this.B * x + this.C * x * x + this.Mu);

            double vxx = 12.0 * x * x - 4.0 + 2.0 * this.C * y * y;
            double vxy = 2.0 * y * (this.B + 2.0 * this.C * x);
            double vyy = 2.0 * (this.B * x + this.C * x * x + this.Mu);

            double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

            Matrix result = new Matrix(2);
            result[0, 0] = (a * vx * vx + vxx);
            result[0, 1] = (a * vx * vy + vxy);
            result[1, 0] = result[0, 1];
            result[1, 1] = (a * vy * vy + vyy);

            return result;
        }

        /// <summary>
        /// One equipotential contour
        /// </summary>
        /// <param name="e">Energy of the system</param>
        /// <param name="n">Number of points in the contour</param>
        /// <param name="x1">Lower bound of the contour</param>
        /// <param name="x2">Upper bound of the contour</param>
        public bool IsConvex(double e, int n) {
            PointVector[] contours = this.EquipotentialContours(e, n);

            for(int i = 0; i < contours.Length; i++) {
                Vector contour = contours[i].VectorY;

                // Second derivative
                bool positive = false;
                bool negative = false;
                for(int j = 2; j < n; j++) {
                    double dd = (contour[j - 2] - 2.0 * contour[j - 1] + contour[j]);
                    if(dd > 0.0)
                        positive = true;
                    if(dd < 0.0)
                        negative = true;
                    if(positive & negative)
                        return false;
                }
            }

            return true;
        }
    }
}
