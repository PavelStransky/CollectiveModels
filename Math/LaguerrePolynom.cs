using System;

namespace PavelStransky.Math {
    /// <summary>
    /// Koeficienty Laguerova polynomu
    /// </summary>
    public class LaguerrePolynom {
        public double[, ,] coefs;     // n, m, i

        /// <summary>
        /// Maximální øád Lagguerova polynomu
        /// </summary>
        public int MaxN { get { return this.coefs.GetLength(0); } }

        /// <summary>
        /// Maximální stupeò Lagguerova polynomu
        /// </summary>
        public int MaxM { get { return this.coefs.GetLength(1); } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="maxN">Maximální øád polynomu</param>
        /// <param name="maxM">Maximální stupeò polynomu</param>
        public LaguerrePolynom(int maxN, int maxM) {
            maxN = System.Math.Max(maxN, 0);
            maxM = System.Math.Max(maxM, 0);

            this.coefs = new double[maxN + 1, maxM + 1, maxN + 1];

            for(int n = 0; n <= maxN; n++)           // Stupeò  Laguerrova polynomu
                for(int m = 0; m <= maxM; m++)
                    for(int k = 0; k <= n; k++)      // Mocnina
                        this.coefs[n, m, k] = (k % 2 == 0 ? 1 : -1) * SpecialFunctions.BinomialCoeficient(n + m, n - k) / SpecialFunctions.Factorial(k);
        }

        /// <summary>
        /// Vrací hodnotu polynomu
        /// </summary>
        /// <param name="n">Øád polynomu</param>
        /// <param name="x">Hodnota x</param>
        public double GetValue(int n, int m, double x) {
            double d = 1;
            double result = 0;

            for(int k = 0; k <= n; k++) {
                result += this.coefs[n, m, k] * d;
                d *= x;
            }

            return result;
        }

        /// <summary>
        /// Statická metoda - nepoužívá cache
        /// </summary>
        /// <param name="n">Øád polynomu</param>
        /// <param name="x">Hodnota x</param>
        public static double GetValueS(int n, int m, double x) {
            double d = 1;
            double result = 0;

            for(int k = 0; k <= n; k++) {
                result += d * (k % 2 == 0 ? 1 : -1) * SpecialFunctions.BinomialCoeficient(n + m, n - k) / SpecialFunctions.Factorial(k);
                d *= x;
            }

            return result;
        }
    }
}
