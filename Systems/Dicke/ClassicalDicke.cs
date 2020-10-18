using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class ClassicalDicke : Dicke, IDynamicalSystem {
        private Random random = new Random();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="omega0">Excitation energy of the atomic part</param>
        /// <param name="omega">Frequency of the radiation mode omega</param>
        /// <param name="gamma">Coupling constant (interaction parameter)</param>
        /// <param name="j">Total angular momentum</param>
        /// <param name="delta">Constant distinguishing between Tavis-Cummings (Delta=0) and Dicke (Delta=1) models</param>
        public ClassicalDicke(double omega0, double omega, double gamma, double j, double delta, double xi, double kappa) : base(omega0, omega, gamma, j, delta, xi, kappa) { }

        public ClassicalDicke(Core.Import import) : base(import) {}

        #region IDynamicalSystem Members
        public double E(Vector x) {
            double q = x[0];
            double phi = x[1];
            double p = x[2];
            double jz = x[3];

            return this.Omega0 * jz + 0.5 * this.Omega * (p * p + q * q)
                + this.Gamma * System.Math.Sqrt(this.J - jz * jz / this.J) * ((1 + this.Delta) * q * System.Math.Cos(phi) - (1 - this.Delta) * p * System.Math.Sin(phi));
        }


        public Matrix Jacobian(Vector x) {
            double q = x[0];
            double phi = x[1];
            double p = x[2];
            double jz = x[3];

            double sqrt = System.Math.Sqrt(this.J - jz * jz / this.J);
            double a = 1.0 / (this.J * sqrt);
            double z = (1.0 + this.Delta) * q * System.Math.Cos(phi) - (1.0 - this.Delta) * p * System.Math.Sin(phi);

            Matrix result = new Matrix(4);

            double dV2pphi = -(1.0 - this.Delta) * this.Gamma * sqrt * System.Math.Cos(phi);
            double dV2pp = this.Omega;
            double dV2pjz = a * (1.0 - this.Delta) * this.Gamma * jz * System.Math.Sin(phi);

            double dV2jzq = -a * (1.0 + this.Delta) * this.Gamma * jz * System.Math.Cos(phi);
            double dV2jzphi = a * this.Gamma * jz * ((1 - this.Delta) * p * System.Math.Cos(phi) + (1 + this.Delta) * q * System.Math.Sin(phi));
            double dV2jzjz = -a * this.Gamma * (jz * jz / (this.J * sqrt * sqrt) + 1.0) * z;

            double dmV2qq = -this.Omega;
            double dmV2qphi = (1.0 + this.Delta) * this.Gamma * sqrt * System.Math.Sin(phi);

            double dmV2phiphi = this.Gamma * sqrt * z;

            result[0, 1] = dV2pphi;
            result[0, 2] = dV2pp;
            result[0, 3] = dV2pjz;

            result[1, 0] = dV2jzq;
            result[1, 1] = dV2jzphi;
            result[1, 2] = result[0, 3];
            result[1, 3] = dV2jzjz;

            result[2, 0] = dmV2qq;
            result[2, 1] = dmV2qphi;
            result[2, 3] = -result[1, 0];

            result[3, 0] = result[2, 1];
            result[3, 1] = dmV2phiphi;
            result[3, 2] = -result[0, 1];
            result[3, 3] = -result[1, 1];

            return result;
        }

        public Vector Equation(Vector x) {
            double q = x[0];
            double phi = x[1];
            double p = x[2];
            double jz = x[3];

            double sqrt = System.Math.Sqrt(this.J - jz * jz / this.J);

            Vector result = new Vector(4);

            result[0] = this.Omega * p - (1.0 - this.Delta) * this.Gamma * sqrt * System.Math.Sin(phi);
            result[1] = this.Omega0
                - (this.Gamma * jz * ((1.0 + this.Delta) * q * System.Math.Cos(phi) - (1.0 - this.Delta) * p * System.Math.Sin(phi))) / (this.J * sqrt);

            result[2] = -this.Omega * q - (1.0 + this.Delta) * this.Gamma * sqrt * System.Math.Cos(phi);
            result[3] = this.Gamma * sqrt * ((1.0 - this.Delta) * p * System.Math.Cos(phi) + (1.0 + this.Delta) * q * System.Math.Sin(phi));

            return result;
        }

        public Vector IC(double e) {
            // Step 1
            if(e < this.EMin)
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
            } while(dbar < 0);  // Step 3

            dbar = System.Math.Sqrt(dbar);

            // Step 4
            double p1 = 0.5 * (-bbar - dbar) / a;
            double p2 = 0.5 * (-bbar + dbar) / a;
            double p = this.random.NextDouble() * (p2 - p1) + p1;

            // Step 5
            double c = -e + a * p * p + this.Omega0 * jz - x * (1.0 - this.Delta) * p * System.Math.Sin(phi);
            double d = System.Math.Sqrt(b * b - 4.0 * a * c);

            double q = 0.0;
            if(this.random.Next(2) == 0)
                q = 0.5 * (-b - d) / a;
            else
                q = 0.5 * (-b + d) / a;

            Vector result = new Vector(4);
            result[0] = q;
            result[1] = phi;
            result[2] = p;
            result[3] = jz;

            double ee = this.E(result);

            return result;
        }

        public bool IC(Vector ic, double e) {
            if(e < this.EMin)
                return false;

            double q = ic[0];
            double phi = ic[1];
            double p = ic[2];
            double jz = ic[3];

            if(double.IsNaN(q)) {
                double x = this.Gamma * System.Math.Sqrt(this.J - jz * jz / this.J);

                double a = 0.5 * this.Omega;
                double b = x * (1.0 + this.Delta) * System.Math.Cos(phi);
                double c = -e + a * p * p + this.Omega0 * jz - x * (1.0 - this.Delta) * p * System.Math.Sin(phi);
                double d = System.Math.Sqrt(b * b - 4.0 * a * c);

                if(this.random.Next(2) == 0)
                    q = 0.5 * (-b - d) / a;
                else
                    q = 0.5 * (-b + d) / a;

                ic[0] = q;

                if(!double.IsNaN(q))
                    return true;
            }

            return false;
        }


        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            if(e <= this.EMin)
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

            if(x[1] > pi) {
                x[1] -= pi2;
                result = true;
            }
            if(x[1] < -pi) {
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
    }
}
