using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class ClassicalDP: DoublePendulum, IDynamicalSystem {
        // Generátor náhodných èísel
        private Random random = new Random();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Pomìr hmotností tìles</param>
        /// <param name="lambda">Pomìr délek</param>
        /// <param name="gamma">Gravitaèní parametr</param>
        public ClassicalDP(double mu, double lambda, double gamma)
            : base(mu, lambda, gamma) { }

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="fi1">Úhel 1</param>
        /// <param name="fi2">Úhel 2</param>
        /// <param name="lambda1">Úhlový moment 1</param>
        /// <param name="lambda2">Úhlový moment 2</param>
        public double T(double fi1, double fi2, double lambda1, double lambda2) {
            double s = System.Math.Sin(fi2);
            double c = System.Math.Cos(fi2);

            double d = 2.0 * (1.0 + this.Mu * s * s);
            double ml2 = this.Mu * this.Lambda * this.Lambda;

            double a1 = lambda1 * lambda1;
            double a2 = 2.0 * lambda1 * lambda2 / this.Lambda * (this.Lambda + c);
            double a3 = lambda2 * lambda2 * (1.0 + this.Mu + 2.0 * this.Mu * this.Lambda * c + ml2) / ml2;

            return (a1 - a2 + a3) / d;
        }

        /// <summary>
        /// Potenciální energie
        /// </summary>
        /// <param name="fi1">Úhel 1</param>
        /// <param name="fi2">Úhel 2</param>
        public double V(double fi1, double fi2) {
            double a1 = (1.0 + this.Mu) * (1.0 - System.Math.Cos(fi1));
            double a2 = this.Mu * this.Lambda * (1.0 - System.Math.Cos(fi1 + fi2));
            return this.Gamma * (a1 + a2);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="fi1">Úhel 1</param>
        /// <param name="fi2">Úhel 2</param>
        /// <param name="lambda1">Úhlový moment 1</param>
        /// <param name="lambda2">Úhlový moment 2</param>
        public double E(double fi1, double fi2, double lambda1, double lambda2) {
            return this.T(fi1, fi2, lambda1, lambda2) + this.V(fi1, fi2);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x[0], x[1], x[2], x[3]) + this.V(x[0], x[1]);
        }

        /// <summary>
        /// Vrátí souøadnice tìlesa 1
        /// </summary>
        /// <param name="fi1">Úhel 1</param>
        public PointD Body1(double fi1) {
            return new PointD(System.Math.Sin(fi1), -System.Math.Cos(fi1));
        }

        /// <summary>
        /// Vrátí souøadnice tìlesa 2
        /// </summary>
        /// <param name="fi1">Úhel 1</param>
        /// <param name="fi2">Úhel 2</param>
        public PointD Body2(double fi1, double fi2) {
            double x = System.Math.Sin(fi1) + this.Lambda * System.Math.Sin(fi1 + fi2);
            double y = -System.Math.Cos(fi1) - this.Lambda * System.Math.Cos(fi1 + fi2);
            return new PointD(x, y);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public Vector Equation(Vector x) {
            Vector result = new Vector(4);

            double fi1 = x[0];
            double fi2 = x[1];
            double lambda1 = x[2];
            double lambda2 = x[3];

            double s1 = System.Math.Sin(fi1);
            double s2 = System.Math.Sin(fi2);
            double c2 = System.Math.Cos(fi2);
            double s12 = System.Math.Sin(fi1 + fi2);

            double glms12 = this.Gamma * this.Mu * this.Lambda * s12;

            double d = 1.0 + this.Mu * s2 * s2;
            double cc = this.Lambda + c2;
            double ml2 = this.Mu * this.Lambda * this.Lambda;

            double a3 = (1.0 + this.Mu + 2.0 * this.Mu * this.Lambda * c2 + ml2) / ml2;
            double b3 = lambda1 * lambda1 - 2.0 * cc * lambda1 * lambda2 / this.Lambda + lambda2 * lambda2 * a3;
            b3 *= this.Mu * c2 * s2 / (d * d);

            result[0] = (lambda1 - cc * lambda2 / this.Lambda) / d;
            result[1] = (-lambda1 * cc / this.Lambda + lambda2 * a3) / d;

            result[2] = -this.Gamma * (1 + this.Mu) * s1 - glms12;
            result[3] = -glms12 - lambda2 * (lambda1 - lambda2) * s2 / (this.Lambda * d) + b3;

            return result;
        }

        /// <summary>
        /// Matice pro výpoèet SALI (Jakobián)
        /// </summary>
        /// <param name="x">Vektor x v èase t</param>
        public Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4);

            double fi1 = x[0];
            double fi2 = x[1];
            double lambda1 = x[2];
            double lambda2 = x[3];

            double s1 = System.Math.Sin(fi1);
            double c1 = System.Math.Cos(fi1);
            double s2 = System.Math.Sin(fi2);
            double c2 = System.Math.Cos(fi2);
            double s12 = System.Math.Sin(fi1 + fi2);
            double c12 = System.Math.Cos(fi1 + fi2);

            double glm = this.Gamma * this.Mu * this.Lambda;
            double gm1 = this.Gamma * (1.0+this.Mu);

            double d = 1.0 + this.Mu * s2 * s2;
            double cc = (this.Lambda + c2)/this.Lambda;
            double ml2 = this.Mu * this.Lambda * this.Lambda;
            double r = 2.0 * this.Mu * s2 * c2 / (d * d);

            double a3 = (1.0 + this.Mu + 2.0 * this.Mu * this.Lambda * c2 + ml2) / ml2;
            double b3 = lambda1 * lambda1 - 2.0 * cc * lambda1 * lambda2 + lambda2 * lambda2 * a3;

            result[0, 1] = lambda2 * s2 / (this.Lambda * d) - r * (lambda1 - cc * lambda2);
            result[0, 2] = 1.0 / d;
            result[0, 3] = -cc / d;

            result[1, 1] = (lambda1 - 2.0 * lambda2) * s2 / (d * this.Lambda) + r * (lambda1 * cc - lambda2 * a3);
            result[1, 2] = result[0, 3];
            result[1, 3] = a3 / d;

            result[2, 0] = -gm1 * c1 - glm * c12;
            result[2, 1] = -glm * c12;

            result[3, 0] = result[2, 1];
            result[3, 1] = -lambda2 * (lambda1 - lambda2) * c2 / (this.Lambda * d)
                + (this.Mu * (c2 * c2 - s2 * s2) / (d * d) - d * r * r) * b3
                + 2.0 * r * lambda2 * s2 * (lambda1 - lambda2) / this.Lambda
                - glm * c12;
            result[3, 2] = -result[0, 1];
            result[3, 3] = -result[1, 1];

            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Konstruktor pro import
        /// </summary>
        /// <param name="import">Import</param>
        public ClassicalDP(Import import) :base(import){}        
        #endregion

        #region IDynamicalSystem Members
        /// <summary>
        /// Poèáteèní podmínky pøi dané energii
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector IC(double e) {
            double fi1, fi2;
            double lambda1, lambda2;

            double v0 = this.Gamma * (1.0 + this.Mu);
            if(e >= 2.0 * v0)
                // fi1 libovolnì v rozmezí -pi ... pi
                fi1 = random.NextDouble() * System.Math.PI;
            else
                fi1 = random.NextDouble() * System.Math.Acos(1.0 - e / v0);
            if(random.Next(2) == 0)
                fi1 = -fi1;

            v0 *= 1.0 - System.Math.Cos(fi1);
            if(e >= v0 + 2.0 * this.Gamma * this.Mu * this.Lambda)
                // fi2 libovolnì v rozmezí -pi ... pi
                fi2 = random.NextDouble() * System.Math.PI;
            else
                fi2 = System.Math.Acos(1.0 - (e - v0) / (this.Gamma * this.Mu * this.Lambda));

            if(random.Next(2) == 0)
                fi2 = -fi2;

            fi2 -= fi1;

            double t = e - this.V(fi1, fi2);
            double td = t * 2.0 * (1.0 + this.Mu * System.Math.Sin(fi2) * System.Math.Sin(fi2));

            double A = 1;
            double B = -2.0 * (this.Lambda + System.Math.Cos(fi2)) / this.Lambda;
            double C = (1.0 + this.Mu + 2.0 * this.Mu * this.Lambda * System.Math.Cos(fi2) + this.Mu * this.Lambda * this.Lambda) / (this.Mu * this.Lambda * this.Lambda);

            // Elipsa se støedem v poèátku, prùseèík s náhodnou pøímkou lambda1 cos(alpha) = lambda2 sin(alpha)
            double alpha = random.NextDouble() * System.Math.PI;
            double talpha = System.Math.Tan(alpha);
            lambda2 = System.Math.Sqrt(td / (A * talpha * talpha + B * talpha + C));
            if(random.Next(2) == 0)
                lambda2 = -lambda2;
            lambda1 = lambda2 * talpha;

            Vector result = new Vector(4);
            result[0] = fi1;
            result[1] = fi2;
            result[2] = lambda1;
            result[3] = lambda2;

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

            double A = 1/ (2.0 * (1.0 + this.Mu));
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

        /// <summary>
        /// Rozhodnutí, zda je daná trajektorie regulární nebo chaotická
        /// </summary>
        /// <param name="meanSALI">Hodnota SALI</param>
        /// <param name="t">Èas</param>
        /// <returns>0 pro chaotickou, 1 pro regulární trajektorii, -1 pro nerozhodnutou</returns>
        public double SALIDecision(double meanSALI, double t) {
            if(meanSALI > 5.0 + t / 200.0)
                return 0;
            if(meanSALI < (t - 500.0) / 50.0)
                return 1;

            return -1;
        }
        #endregion
    }
}
