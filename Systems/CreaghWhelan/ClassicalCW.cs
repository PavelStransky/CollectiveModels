using System;
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

        public double V(double x, double y) {
            double x2 = x * x;
            double y2 = y * y;
            double p = x2 - 1.0;

            double bracket = p * p;
            if(this.Power == 4)
                bracket *= bracket;
            else if(this.Power == -4)
                bracket = bracket + bracket * bracket;

            return bracket + this.Mu * y2 + this.C * x2 * y2 + this.A * x + this.B * x * y2;
        }

        public double E(Vector x) {
            return this.T(x[2], x[3]) + this.V(x[0], x[1]);
        }

        public Matrix Jacobian(Vector x) {
            throw new Exception("The method or operation is not implemented.");
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

        private class BisectionPotential {
            private ClassicalCW cg;
            private double x, y, e;

            public BisectionPotential(ClassicalCW cg, double x, double y, double e) {
                this.cg = cg;
                this.x = x;
                this.y = y;
                this.e = e;
            }

            public double BisectionX(double x) {
                return cg.V(x, this.y) - e;
            }

            public double BisectionY(double y) {
                return cg.V(this.x, y) - e;
            }
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
            throw new Exception("The method or operation is not implemented.");
        }

        private double VX(double x) {
            return this.A + 4.0 * x * (x * x - 1);
        }

        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            result[0] = -2;
            result[1] = 2;
            result[2] = -2;
            result[3] = 2;
            result[4] = -2;
            result[5] = 2;
            result[6] = -2;
            result[7] = 2;

            return result;

            BisectionPotential bp = new BisectionPotential(this, 0.0, 0.0, e);
            Bisection b = new Bisection(bp.BisectionX);

            // x
            if(e <= 1) {
                double x1 = b.Solve(-10 - System.Math.Abs(this.A), -1);
                double x2 = b.Solve(-1, 0);
                double x3 = b.Solve(0, 1);
                double x4 = b.Solve(1, 10 + System.Math.Abs(this.A));

                if(!double.IsNaN(x1))
                    result[0] = x1;
                else if(!double.IsNaN(x2))
                    result[0] = x2;
                else result[0] = x3;

                if(!double.IsNaN(x4))
                    result[1] = x4;
                else if(!double.IsNaN(x3))
                    result[1] = x3;
                else result[1] = x2;
            }
            else {
                result[0] = b.Solve(-10 - e, 0);
                result[1] = b.Solve(0, e + 10);                
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

        public Vector CheckBounds(Vector bounds) {
            throw new Exception("The method or operation is not implemented.");
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
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public ClassicalCW(Core.Import import) : base(import) { }
    }
}
