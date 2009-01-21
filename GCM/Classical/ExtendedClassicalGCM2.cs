using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.GCM {
    /// <summary>
    /// Klasický GCM s kinetickým èlenem v Hamiltoniánu úmìrným beta^2
    /// </summary>
    public class ExtendedClassicalGCM2 : ExtendedClassicalGCM1 {
        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public override double T(double x, double y, double px, double py) {
            double b2 = x * x + y * y;
            return 1.0 / (2.0 * this.K) * (this.Lambda + this.Kappa * b2) * (px * px + py * py);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public override Vector Equation(Vector x) {
            Vector result = new Vector(4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dVdx = 2.0 * this.A * x[0] + 3.0 * this.B * (x[0] * x[0] - x[1] * x[1]) + 4.0 * this.C * x[0] * b2;
            double dVdy = 2.0 * x[1] * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * b2);

            double e = 1.0 / this.K * (this.Lambda + this.Kappa * b2);
            double f = -this.Kappa / this.K * (x[2] * x[2] + x[3] * x[3]);

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
        public override Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4, 4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dV2dxdx = 2.0 * this.A + 6.0 * this.B * x[0] + 4.0 * this.C * (3.0 * x[0] * x[0] + x[1] * x[1]);
            double dV2dxdy = (-6.0 * this.B + 8.0 * this.C * x[0]) * x[1];
            double dV2dydy = 2.0 * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * (x[0] * x[0] + 3.0 * x[1] * x[1]));

            double e = 1.0 / this.K * (this.Lambda + this.Kappa * b2);
            double f = -this.Kappa / this.K * (x[2] * x[2] + x[3] * x[3]);
            double g = 2 * this.Kappa / this.K;

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
        /// Konstruktor rozšíøeného Lagrangiánu
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr K</param>
        /// <param name="kappa">Parametr kappa</param>
        /// <param name="lambda">Parametr Lambda</param>
        public ExtendedClassicalGCM2(double a, double b, double c, double k, double kappa, double lambda)
            : base(a, b, c, k, kappa, lambda) {}

        #region Implementace IExportable
        /// <summary>
        /// Naète GCM tøídu ze souboru textovì
        /// </summary>
        /// <param name="import">Import</param>
        public ExtendedClassicalGCM2(Core.Import import)
        : base(import) {}
        #endregion
    }
}