using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Klasický GCM s kinetickým èlenem v Hamiltoniánu úmìrným beta^2
    /// </summary>
    public class ExtendedClassicalGCM2 : GCM, IDynamicalSystem {
        // Generátor náhodných èísel
        private Random random = new Random();

        // Rozšíøený parametr
        private double lambda;
        public double Lambda { get { return this.lambda; } set { this.lambda = value; } }

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public double T(double x, double y, double px, double py) {
            double b2 = x * x + y * y;
            return 1.0 / (2.0 * this.K) * (1 + this.Lambda * b2) * (px * px + py * py);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="px">Souøadnice x</param>
        /// <param name="py">Souøadnice y</param>
        public double E(double x, double y, double px, double py) {
            return this.T(x, y, px, py) + this.V(x, y);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x[0], x[1], x[2], x[3]) + this.V(x[0], x[1]);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public Vector Equation(Vector x) {
            Vector result = new Vector(4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dVdx = 2.0 * this.A * x[0] + 3.0 * this.B * (x[0] * x[0] - x[1] * x[1]) + 4.0 * this.C * x[0] * b2;
            double dVdy = 2.0 * x[1] * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * b2);

            double e = 1.0 / this.K * (1 + this.Lambda * b2);
            double f = -this.Lambda / this.K * (x[2] * x[2] + x[3] * x[3]);

            result[0] = e * x[2];
            result[1] = e * x[3];

            result[2] = f * x[0] - dVdx;
            result[3] = f * x[1] - dVdy;

            return result;
        }

        /// <summary>
        /// Matice pro výpoèet SALI (Jakobián)
        /// </summary>
        /// <param name="x">Vektor x v èase t</param>
        public Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4, 4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dV2dxdx = 2.0 * this.A + 6.0 * this.B * x[0] + 4.0 * this.C * (3.0 * x[0] * x[0] + x[1] * x[1]);
            double dV2dxdy = (-6.0 * this.B + 8.0 * this.C * x[0]) * x[1];
            double dV2dydy = 2.0 * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * (x[0] * x[0] + 3.0 * x[1] * x[1]));

            double e = 1.0 / this.K * (1 + this.Lambda * b2);
            double f = -this.Lambda / this.K * (x[2] * x[2] + x[3] * x[3]);
            double g = 2 * this.Lambda / this.K;

            result[0, 0] = g * x[0] * x[2];
            result[0, 1] = g * x[1] * x[2];
            result[0, 2] = e;

            result[1, 0] = g * x[0] * x[3];
            result[1, 1] = g * x[1] * x[3];
            result[1, 3] = e;

            result[2, 0] = f - dV2dxdx;
            result[2, 1] = - dV2dxdy;
            result[2, 2] = -result[0, 0];
            result[2, 3] = -result[1, 0];

            result[3, 0] = result[2, 1];
            result[3, 1] = f - dV2dydy;
            result[3, 2] = -result[0, 1];
            result[3, 3] = -result[1, 1];

            return result;
        }

        /// <summary>
        /// Poèet stupòù volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// Konstruktor rozšíøeného Lagrangiánu
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr K</param>
        /// <param name="lambda">Parametr Lambda</param>
        public ExtendedClassicalGCM2(double a, double b, double c, double k, double lambda)
            : base(a, b, c, k) {
            this.lambda = lambda;
        }

        /// <summary>
        /// Generuje poèáteèní podmínky rovnomìrnì ve FP
        /// </summary>
        /// <param name="e">Energie</param>
        /// <returns>Poèáteèní podmínky ve formátu (x, y, px, py)</returns>
        public Vector IC(double e) {
            Vector result = new Vector(4);

            Vector r = this.Roots(e, 0);
            if(r.Length == 0)
                throw new GCMException(string.Format(errorMessageInitialCondition, e));

            // Nalezení nejvìtšího koøenu (v absolutní hodnotì)
            double rmax = System.Math.Abs(r[0]);
            for(int i = 1; i < r.Length; i++)
                if(System.Math.Abs(r[i]) > rmax)
                    rmax = System.Math.Abs(r[i]);
            do {
                // Poèáteèní podmínky v poloze hledáme ve èverci (-rmax, rmax) x (-rmax, rmax)
                result[0] = (this.random.NextDouble() * 2.0 - 1) * rmax;
                result[1] = (this.random.NextDouble() * 2.0 - 1) * rmax;

                double b2 = result[0] * result[0] + result[1] * result[1];
                double d = 1.0 / this.K * (1 + this.Lambda * b2);
                double tbracket = 2.0 * (e - this.V(result[0], result[1])) * d;

                if(tbracket < 0)
                    continue;

                result[2] = this.random.NextDouble() * System.Math.Sqrt(tbracket);
                result[3] = System.Math.Sqrt(tbracket - result[2] * result[2]);

                if(this.random.Next(2) == 0)
                    result[2] = -result[2];
                if(this.random.Next(2) == 0)
                    result[3] = -result[3];

                result[2] /= d;
                result[3] /= d;

                //              double chkE = this.E(result);

                break;
            } while(true);

            return result;
        }

        /// <summary>
        /// Generuje poèáteèní podmínky ve FP
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">Impulsmoment</param>
        public Vector IC(double e, double l) {
            if(l == 0.0)
                return this.IC(e);
            else
                throw new GCMException(string.Format(errorMessageNonzeroJ, this.GetType().FullName, typeof(ClassicalGCMJ).FullName));
        }

        /// <summary>
        /// Vypíše parametry GCM modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("A = {0,10:#####0.000}\nB = {1,10:#####0.000}\nC = {2,10:#####0.000}\nK = {3,10:#####0.000}\nlambda = {4,10:#####0.000}", this.A, this.B, this.C, this.K, this.Lambda));
            s.Append(string.Format("I = {0,10:#####0.000}", this.Invariant));
            s.Append("\n\n");

            Vector beta = this.ExtremalBeta(0.0);
            s.Append(string.Format("Extrémy:"));

            for(int i = 0; i < beta.Length; i++)
                s.Append(string.Format("\nV({0,1:0.000}) = {1,1:0.000}", beta[i], this.VBG(beta[i], 0.0)));

            return s.ToString();
        }

        private const double poincareTime = 100;
        private const int degreesOfFreedom = 2;

        private const string errorMessageInitialCondition = "Pro zadanou energii {0} nelze nagenerovat poèáteèní podmínky.";
        private const string errorMessageNonzeroJ = "Tøída {0} umí poèítat pouze s nulovým úhlovým momentem. Pro nenulový úhlový moment použij {1}.";
    }
}