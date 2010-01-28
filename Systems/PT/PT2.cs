using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.PT {
    /// <summary>
    /// System for studying quantum phase transitions
    /// </summary>
    public class PT2: PT1 {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected PT2() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="omega0">Basis frequency</param>
        /// <param name="m">Mixing parameter</param>
        /// <param name="hbar">Planck constant</param>
        public PT2(double m, double omega0, double hbar) : base(m, omega0, hbar) { }

        public PT2(Core.Import import) : base(import) { }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici (v tomto pøípadì lze poèítat algebraicky)
        /// </summary>
        /// <param name="maxn">Nejvyšší hodnota kvantového èísla (energie)</param>
        public override SymmetricBandMatrix HamiltonianSBMatrix(int maxn) {
            SymmetricBandMatrix matrix = new SymmetricBandMatrix(maxn, 4);

            double alpha2 = this.omega0 / this.hbar;
            double alpha = System.Math.Sqrt(alpha2);
            double alpha4 = alpha2 * alpha2;

            double a = 10.0 * System.Math.Abs(m) - 2.0 - 0.5 * this.omega0 * this.omega0;

            // Temporary variables
            double r = this.hbar * this.omega0;
            double s = 0.5 * (1.0 - System.Math.Abs(this.m)) / alpha4;
            double t = 0.5 * a / alpha2;
            double u = 7.0 * System.Math.Abs(this.m) + 1.0;
            double v = 16.0 * this.m / alpha;

            double rsqr2 = 1.0 / System.Math.Sqrt(2.0);

            for(int i = 0; i < maxn; i++) {
                double i1 = System.Math.Sqrt(i + 1);
                double i2 = i1 * System.Math.Sqrt(i + 2);
                double i3 = i2 * System.Math.Sqrt(i + 3);
                double i4 = i3 * System.Math.Sqrt(i + 4);

                matrix[i, i] = r * (i + 0.5) + 3.0 * s * (i * i + i + 0.5) + t * (2 * i + 1) + u;
                if(i + 1 < maxn)
                    matrix[i, i + 1] = i1 * rsqr2 * v;
                if(i + 2 < maxn)
                    matrix[i, i + 2] = i2 * (s * (2.0 * i + 3.0) + t);
                if(i + 4 < maxn)
                    matrix[i, i + 4] = 0.5 * s * i4;
            }

            return matrix;
        }

        /// <summary>
        /// Potential
        /// </summary>
        /// <param name="m">Mixing parameter</param>
        public static new double V(double x, double m) {
            double result = m > 0.0 ? m * 8.0 * (x - 1) * (x - 1) : -m * 8.0 * (x + 1) * (x + 1);
            result += (x * x * x * x - 2.0 * x * x + 1.0) * (1.0 - System.Math.Abs(m));
            return result;
        }
    }
}