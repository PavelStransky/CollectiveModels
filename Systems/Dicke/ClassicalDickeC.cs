using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class ClassicalDickeC : Dicke, IDynamicalSystem {
        private Random random = new Random();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="omega0">Excitation energy of the atomic part</param>
        /// <param name="omega">Frequency of the radiation mode omega</param>
        /// <param name="gamma">Coupling constant (interaction parameter)</param>
        /// <param name="j">Total angular momentum</param>
        /// <param name="delta">Constant distinguishing between Tavis-Cummings (Delta=0) and Dicke (Delta=1) models</param>
        /// <param name="xi">Constant for the Rabi model (for Dicke and TC xi = 1)</param>
        public ClassicalDickeC(double omega0, double omega, double gamma, double j, double delta, double xi, double kappa) : base(omega0, omega, gamma, j, delta, xi, kappa) { }

        public ClassicalDickeC(Core.Import import) : base(import) { }

        private Vector InverseCanonical(Vector x) {
            Vector result = new Vector(4);

            double q = x[0];
            double p = x[2];

            double phi = x[1] - x[3];

            result[1] = phi;
            result[3] = x[3] - 0.5 * (q * q + p * p);

            result[0] = q * System.Math.Cos(phi) + p * System.Math.Sin(phi);
            result[2] = p * System.Math.Cos(phi) - q * System.Math.Sin(phi);

            return result;
        }

        private Vector Canonical(Vector x) {
            Vector result = new Vector(4);

            double q = x[0];
            double p = x[2];

            double phi = x[1];
            double m = x[3] + 0.5 * (q * q + p * p);

            result[0] = q * System.Math.Cos(phi) - p * System.Math.Sin(phi);
            result[2] = p * System.Math.Cos(phi) + q * System.Math.Sin(phi);


            result[1] = phi + m;
            result[3] = m;

            return result;
        }

        #region IDynamicalSystem Members
        public double E(Vector x) {
            double q = x[0];
            double phi = x[1];
            double p = x[2];
            double m = x[3];

            double pq = 0.5 * (p * p + q * q);
            double bra = m - pq;
            double sqrt = System.Math.Sqrt(this.J - bra * bra / this.J);

            double s = System.Math.Sin(2 * (phi - m));
            double c = System.Math.Cos(2 * (phi - m));

            double a = q + this.Delta * (q * c + p * s);

            return (this.Omega - this.Omega0) * pq + this.Omega0 * m + this.Gamma * sqrt * a;
        }

        public Vector Equation(Vector x) {
            double q = x[0];
            double phi = x[1];
            double p = x[2];
            double m = x[3];

            double pq = 0.5 * (p * p + q * q);
            double bra = m - pq;
            double sqrt = System.Math.Sqrt(this.J - bra * bra / this.J);

            double s = System.Math.Sin(2 * (phi - m));
            double c = System.Math.Cos(2 * (phi - m));

            double a = q + this.Delta * (q * c + p * s);
            double b = this.Delta * this.Gamma * sqrt;
            double d = (this.Gamma * bra / this.J) * a / sqrt;
            double e = 2.0 * b * (q * s - p * c);

            double domega = this.Omega - this.Omega0;

            Vector result = new Vector(4);

            result[0] = domega * p + b * s + d * p;
            result[1] = this.Omega0 + e - d;
            result[2] = -domega * q - this.Gamma * sqrt * (1 + this.Delta * c) - d * q;
            result[3] = e;

            return result;
        }

        public Vector IC(double e) {
            // Step 1
            if (e < this.EMin)
                return null;

            double a = 0.0;
            double b = 0.0;
            double x = 0.0;
            double jz = 0.0;
            double phi = 0.0;
            double bbar = 0.0;
            double cbar = 0.0;
            double dbar = 0.0;

            // Step 2
            do {
                jz = 2.0 * this.J * this.random.NextDouble() - this.J;
                phi = this.random.NextDouble() * System.Math.PI;

                x = this.Gamma * System.Math.Sqrt(this.J - jz * jz / this.J);

                a = 0.5 * this.Omega;
                b = x * (1.0 + this.Delta) * System.Math.Cos(phi);

                bbar = -x * (1.0 - this.Delta) * System.Math.Sin(phi);
                cbar = -e + this.Omega0 * jz - 0.5 * b * b / this.Omega;

                dbar = bbar * bbar - 4.0 * a * cbar;
            } while (dbar < 0);  // Step 3

            dbar = System.Math.Sqrt(dbar);

            // Step 4
            double p1 = 0.5 * (-bbar - dbar) / a;
            double p2 = 0.5 * (-bbar + dbar) / a;
            double p = this.random.NextDouble() * (p2 - p1) + p1;

            // Step 5
            double c = -e + a * p * p + this.Omega0 * jz - x * (1.0 - this.Delta) * p * System.Math.Sin(phi);
            double d = System.Math.Sqrt(b * b - 4.0 * a * c);

            double q = 0.0;
            if (this.random.Next(2) == 0)
                q = 0.5 * (-b - d) / a;
            else
                q = 0.5 * (-b + d) / a;

            Vector result = new Vector(4);
            result[0] = q;
            result[1] = phi;
            result[2] = p;
            result[3] = jz;

            Vector tmp = this.InverseCanonical(this.Canonical(result));

            return this.Canonical(result);
        }

        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            if (e <= this.EMin)
                return result;

            result[0] = -10;
            result[1] = 10;
            result[2] = -System.Math.PI;
            result[3] = System.Math.PI;

            result[4] = -10;
            result[5] = 10;
            result[6] = -this.J;
            result[7] = this.J;

            return result;
        }

        public Vector CheckBounds(Vector bounds) {
            bounds[2] = System.Math.Max(bounds[0], -System.Math.PI);
            bounds[3] = System.Math.Min(bounds[1], System.Math.PI);
            bounds[6] = System.Math.Max(bounds[2], -this.J);
            bounds[7] = System.Math.Min(bounds[3], this.J);
            return bounds;
        }

        public int DegreesOfFreedom {
            get { return 2; }
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            double pi = System.Math.PI;
            double pi2 = 2.0 * pi;
            bool result = false;

            if (x[1] > pi) {
                x[1] -= pi2;
                result = true;
            }
            if (x[1] < -pi) {
                x[1] += pi2;
                result = true;
            }

            return result;
        }

        public double[] SALIDecisionPoints() {
            return new double[] { 0, 8, 2000, 0, 4000, 12 };
        }
        #endregion

        public Vector IC(double e, double l) {
            throw new NotImplementedException();
        }

        public double PeresInvariant(Vector x) {
            throw new NotImplementedException();
        }


        public Matrix Jacobian(Vector x) {
            throw new NotImplementedException();
        }

        public bool IC(Vector ic, double e) {
            throw new NotImplementedException();
        }
    }
}
