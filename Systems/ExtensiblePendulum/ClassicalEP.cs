using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;
/*
namespace PavelStransky.Systems {
    public class ClassicalEP: ExtensiblePendulum, IDynamicalSystem {
        // Generátor náhodných èísel
        private Random random = new Random();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="nu">Parametr modelu</param>
        public ClassicalEP(double nu)
            : base(nu) { }

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public double T(double px, double py) {
            return 0.5 * (px * px + py * py);
        }

        /// <summary>
        /// Potenciální energie
        /// </summary>
        /// <param name="x">Horizontální souøadnice</param>
        /// <param name="y">Vertikální souøadnice</param>
        public double V(double x, double y) {
            double d = System.Math.Sqrt(x * x + y * y) - 1.0;
            return this.Nu * y + 0.5 * d * d;
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        /// <param name="x">Horizontální souøadnice</param>
        /// <param name="y">Vertikální souøadnice</param>
        public double E(double x, double y, double px, double py) {
            return this.T(px, py) + this.V(x, y);
        }

        /// <summary>
        /// Minimální energie (kyvadlo visí dolù, délka je L)
        /// </summary>
        public double Emin() {
            return -this.Nu * (this.Nu / 2.0 + 1.0);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x[2], x[3]) + this.V(x[0], x[1]);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="v">Souøadnice a hybnosti</param>
        public Vector Equation(Vector v) {
            Vector result = new Vector(4);

            double x = v[0];
            double y = v[1];

            double r = System.Math.Sqrt(x * x + y * y);
            double d = 1.0 - 1.0 / r;

            double dVdx = x * d;
            double dVdy = this.Nu + y * d;

            result[0] = v[2];
            result[1] = v[3];

            result[2] = dVdx;
            result[3] = dVdy;

            return result;
        }

        /// <summary>
        /// Matice pro výpoèet SALI (Jakobián)
        /// </summary>
        /// <param name="x">Vektor x v èase t</param>
        public Matrix Jacobian(Vector v) {
            Matrix result = new Matrix(4);

            double x = v[0];
            double y = v[1];

            double r = System.Math.Sqrt(x * x + y * y);
            double r3 = r * r * r;

            double dV2dxdx = 1.0 - y * y / r3;
            double dV2dxdy = x * y / r3;
            double dV2dydy = 1.0 - x * x / r3;

            result[0, 2] = 1.0;
            result[1, 3] = result[0, 2];

            result[2, 0] = -dV2dxdx;
            result[2, 1] = -dV2dxdy;

            result[3, 0] = result[2, 1];
            result[3, 1] = -dV2dydy; 
            
            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Konstruktor pro import
        /// </summary>
        /// <param name="import">Import</param>
        public ClassicalEP(Import import) : base(import) { }
        #endregion

        #region IDynamicalSystem Members
        /// <summary>
        /// Poèáteèní podmínky pøi dané energii
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector IC(double e) {
            Vector result = new Vector(4);

            // Nejlepší je generovat poèáteèní podmínky v rovinì r, phi (polární souøadnice)
            double phi = 2.0 * System.Math.PI * this.random.NextDouble();

            // Nalezení nejvìtšího koøenu (v absolutní hodnotì)
            double rmax = System.Math.Abs(r[0]);
            for(int i = 1; i < r.Length; i++)
                if(System.Math.Abs(r[i]) > rmax)
                    rmax = System.Math.Abs(r[i]);
            do {
                // Poèáteèní podmínky v poloze hledáme ve èverci (-rmax, rmax) x (-rmax, rmax)
                result[0] = (this.random.NextDouble() * 2.0 - 1) * rmax;
                result[1] = (this.random.NextDouble() * 2.0 - 1) * rmax;

                result[2] = 0.0;
                result[3] = 0.0;

                if(this.E(result) < e) {
                    result[2] = double.NaN;
                    result[3] = double.NaN;

                    if(this.IC(result, e))
                        break;
                }

            } while(true);

            return result;
        }

        /// <summary>
        /// Poèáteèní podmínky pøi dané energii; dopoèítáváme jen hodnoty, které jsou double.NaN
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="ic">Poèáteèní podmínky</param>
        /// <returns>True, pokud se poèáteèní podmínky podaøilo nagenerovat</returns>
        public bool IC(Vector ic, double e) {
            double fi1 = ic[0];
            double fi2 = ic[1];
            double lambda1 = ic[2];
            double lambda2 = ic[3];

            double t = e - this.V(fi1, fi2);
            if(t < 0)
                return false;

            double v0 = this.Gamma * (1.0 + this.Mu);
            double td = t * 2.0 * (1.0 + this.Mu * System.Math.Sin(fi2) * System.Math.Sin(fi2));

            double A = 1;
            double B = -2.0 * (this.Lambda + System.Math.Cos(fi2)) / this.Lambda;
            double C = (1.0 + this.Mu + 2.0 * this.Mu * this.Lambda * System.Math.Cos(fi2) + this.Mu * this.Lambda * this.Lambda) / (this.Mu * this.Lambda * this.Lambda);

            // Generujeme oba impulsmomenty najednou
            if(double.IsNaN(lambda1) && double.IsNaN(lambda2)) {
                // Elipsa se støedem v poèátku, prùseèík s náhodnou pøímkou lambda1 cos(alpha) = lambda2 sin(alpha)
                double alpha = random.NextDouble() * System.Math.PI;
                double talpha = System.Math.Tan(alpha);
                lambda2 = System.Math.Sqrt(td / (A * talpha * talpha + B * talpha + C));
                if(random.Next(2) == 0)
                    lambda2 = -lambda2;
                lambda1 = lambda2 * talpha;
            }
            else if(double.IsNaN(lambda1)) {
                lambda1 = 4.0 * A * td + lambda2 * lambda2 * (B * B - 4.0 * A * C);
                if(lambda1 < 0)
                    return false;
                lambda1 = System.Math.Sqrt(lambda1);
                if(random.Next(2) == 0)
                    lambda1 = -lambda1;
                lambda1 -= B * lambda2;
                lambda1 /= 2.0 * A;
            }
            else if(double.IsNaN(lambda2)) {
                lambda2 = 4.0 * C * td + lambda1 * lambda1 * (B * B - 4.0 * A * C);
                if(lambda2 < 0)
                    return false;
                lambda2 = System.Math.Sqrt(lambda2);
                if(random.Next(2) == 0)
                    lambda2 = -lambda2;
                lambda2 -= B * lambda1;
                lambda2 /= 2.0 * C;
            }

            ic[2] = lambda1;
            ic[3] = lambda2;

            double ee = this.E(ic);

            return true;
        }

        public Vector IC(double e, double l) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Meze dynamických promìnných pøi dané energii
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            double c1 = this.Gamma * (1.0 + this.Mu);
            double c2 = this.Gamma * this.Mu * this.Lambda;

            // fi1
            if(e >= 2.0 * c1) {
                result[0] = -System.Math.PI;
                result[1] = System.Math.PI;
            }
            else {
                double d = System.Math.Acos(1.0 - e / c1);
                result[0] = -d;
                result[1] = d;
            }

            // fi2
            if(e >= 2.0 * c2) {
                result[2] = -System.Math.PI;
                result[3] = System.Math.PI;
            }
            else {
                double d = System.Math.Acos(1.0 - e / c2);
                result[2] = -d;
                result[3] = d;
            }

            double A = 1 / (2.0 * (1.0 + this.Mu));
            double B = -2.0 * (this.Lambda + 1.0) / this.Lambda / (2.0 * (1.0 + this.Mu));
            double C = (1.0 + this.Mu + 2.0 * this.Mu * this.Lambda + this.Mu * this.Lambda * this.Lambda) / (this.Mu * this.Lambda * this.Lambda) / (2.0 * (1.0 + this.Mu));

            double sqrt = System.Math.Sqrt(4.0 * A * C - B * B);

            // lambda1
            result[4] = -2.0 * System.Math.Sqrt(C * e) / sqrt;
            result[5] = -result[4];

            // lambda2
            result[6] = -2.0 * System.Math.Sqrt(A * e) / sqrt;
            result[7] = -result[6];

            return result;
        }

        /// <summary>
        /// Kontrola poèáteèních podmínek
        /// </summary>
        /// <param name="bounds">Poèáteèní podmínky</param>
        public Vector CheckBounds(Vector bounds) {
            // poèáteèní podmínky ve smìru x, y pouze v rozmezí (-pi, pi)
            bounds[0] = System.Math.Max(bounds[0], -System.Math.PI);
            bounds[1] = System.Math.Min(bounds[1], System.Math.PI);
            bounds[2] = System.Math.Max(bounds[2], -System.Math.PI);
            bounds[3] = System.Math.Min(bounds[3], System.Math.PI);

            return bounds;
        }

        public int DegreesOfFreedom { get { return 2; } }

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            double pi = System.Math.PI;
            double pi2 = 2.0 * pi;
            bool result = false;

            if(x[0] > pi) {
                x[0] -= pi2;
                result = true;
            }
            if(x[0] < -pi) {
                x[0] += pi2;
                result = true;
            }
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

        #endregion
    }
}
*/