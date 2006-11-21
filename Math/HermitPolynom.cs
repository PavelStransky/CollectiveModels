using System;

namespace PavelStransky.Math {
    /// <summary>
    /// Koeficienty Hermitova polynomu
    /// </summary>
    /// <remarks>Petr</remarks>
    public class HermitPolynom {
        private Matrix coefs;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxOrder">ÿ·d polynomu</param>
        public HermitPolynom(int maxOrder) {
            maxOrder = System.Math.Max(maxOrder, 1);

            this.coefs = new Matrix(maxOrder + 1);
            this.coefs[0, 0] = 1;
            this.coefs[1, 0] = 0;
            this.coefs[1, 1] = 2;

            for(int n = 2; n <= maxOrder; n++) {
                this.coefs[n, 0] = -2 * (n - 1) * this.coefs[n - 2, 0];

                for(int i = 1; i <= maxOrder; i++)
                    this.coefs[n, i] = 2 * this.coefs[n - 1, i - 1] - 2 * (n - 1) * this.coefs[n - 2, i];
            }
        }

        /// <summary>
        /// VracÌ hodnotu polynomu
        /// </summary>
        /// <param name="n">ÿ·d polynomu</param>
        /// <param name="x">Hodnota x</param>
        public double GetValue(int n, double x) {
            double m = 1;
            double result = 0;

            for(int i = 0; i <= n; i++) {
                result += this.coefs[n, i] * m;
                m *= x;
            }

            return result;
        }
    }
}
