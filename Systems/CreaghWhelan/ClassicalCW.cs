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

            // x
            if (e <= 1) {
                double x1 = b.Solve(-10 - System.Math.Abs(this.A), -1);
                double x2 = b.Solve(-1, 0);
                double x3 = b.Solve(0, 1);
                double x4 = b.Solve(1, 10 + System.Math.Abs(this.A));

                if (!double.IsNaN(x1) && !double.IsNaN(x2)) {
                    if (!double.IsNaN(x3) && !double.IsNaN(x4) && random.Next(2) == 0)
                        result[0] = random.NextDouble() * (x4 - x3) + x3;
                    else
                        result[0] = random.NextDouble() * (x2 - x1) + x1;
                }
                else
                    result[0] = random.NextDouble() * (x4 - x3) + x3;
            }
            else {
                double x1 = b.Solve(-10 - e, 0);
                double x2 = b.Solve(0, e + 10);
                result[0] = random.NextDouble() * (x2 - x1) + x1;
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

        public Vector Bounds(double e) {
            Vector result = new Vector(8);

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

            // Minima
            Vector vext = new Vector(ext.Count);
            for(int i = 0; i < ext.Count; i++)
                vext[i] = this.V((double)ext[i], 0.0);
            double min = (double)ext[vext.MinIndex()];
            double vmin = vext.Min();

            // Solutions
            BisectionPotential bp = new BisectionPotential(this, 0.0, 0.0, e);
            b = new Bisection(bp.BisectionX);

            ArrayList sol = new ArrayList();

            x = b.Solve(-10 - System.Math.Abs(this.A), (double)ext[0]);
            if(!double.IsNaN(x))
                sol.Add(x);
            for(int i = 0; i < ext.Count - 1; i++) {
                x = b.Solve((double)ext[i], (double)ext[i + 1]);
                if(!double.IsNaN(x))
                    sol.Add(x);
            }
            x = b.Solve((double)ext[ext.Count - 1], 10 + System.Math.Abs(this.A));
            if(!double.IsNaN(x))
                sol.Add(x);

            result[0] = (double)sol[0];
            result[1] = (double)sol[sol.Count - 1];

            double pisvejc = 1E-6;

            // y
            double minxy = min;
            double maxxy = minxy < 0 ? System.Math.Min((double)sol[1] - pisvejc, 0.0) : System.Math.Max((double)sol[sol.Count - 2] + pisvejc, 0.0);

            BisectionY by = new BisectionY(this, e);
            b = new Bisection(by.Bisection);
            double y1 = b.Minimum(System.Math.Min(minxy, maxxy), System.Math.Max(minxy, maxxy));

            double y2 = 0.0;
            if(minxy < 0.0 && result[1] > 0.0)
                y2 = b.Minimum(System.Math.Max(0.0, (double)sol[sol.Count - 2] + pisvejc), result[1]);
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

        public int SALIDecision(double meanSALI, double t) {
            if(meanSALI > 6.0 + t / 200.0)
                return 0;
            if(meanSALI < (t - 500.0) / 50.0)
                return 1;

            return -1;
        }
        #endregion

        public ClassicalCW(Core.Import import) : base(import) { }
    }
}
