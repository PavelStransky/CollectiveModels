using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Tøída generující Gaussovo náhodné rozdìlení
	/// f(x) = 1 / (sqrt(2 pi) sigma) exp(-(x - mi)^2 / (2 sigma^2))
	/// </summary>
	public class NormalDistribution {
		/// <summary>
		/// Generátor rovnomìrnì rozdìlených náhodných èísel
		/// </summary>
		private Random r;

		/// <summary>
		/// Konstruktor (pro výpoèet box-mullerovou metodou)
		/// </summary>
		public NormalDistribution() {
			this.r = new Random();
		}

        /// <summary>
        /// Vrátí jednu hodnotu se standardním rozptylem a støední hodnotou
        /// </summary>
        public double GetValue() {
            return this.GetValue(1.0, 0.0);
        }

        /// <summary>
        /// Vrátí jednu hodnotu
        /// </summary>
        /// <param name="variance">Rozptyl rozdìlení</param>
        /// <param name="mean">Støední hodnota rozdìlení</param>
        public double GetValue(double variance, double mean, int num = 0) {
            if (num <= 0) {
                double n1, n2;
                this.BoxMuller(out n1, out n2, variance, mean);

                return n1;
            }
            else {
                double result = 0;
                for (int i = 0; i < num; i++)
                    result += this.r.NextDouble();
                result -= 0.5 * num;
                result *= variance * 12 / num;
                return result;
            }
        }

        /// <summary>
        /// Vrátí vektor se standardním rozptylem a støední hodnotou
        /// </summary>
        public Vector GetVector(int length) {
            return this.GetVector(length, 1.0, 0.0);
        }

		/// <summary>
		/// Vrátí vektor
		/// </summary>
		/// <param name="length">Délka vektoru</param>
        /// <param name="variance">Rozptyl rozdìlení</param>
        /// <param name="mean">Støední hodnota rozdìlení</param>
        public Vector GetVector(int length, double variance, double mean, int num = 0) {
            Vector result = new Vector(length);

            if (num <= 0) {
                double n1, n2;

                for (int i = 0; i < length / 2; i++) {
                    this.BoxMuller(out n1, out n2, variance, mean);
                    result[2 * i] = n1;
                    result[2 * i + 1] = n2;
                }

                // Pokud je poèet prvkù lichý, vyplníme ještì poslední hodnotu
                if (length % 2 > 0) {
                    this.BoxMuller(out n1, out n2, variance, mean);
                    result[length - 1] = n1;
                }
            }
            else
                for (int i = 0; i < length; i++)
                    result[i] = this.GetValue(variance, mean);

            return result;
        }

        /// <summary>
        /// Box - Mullerova metoda generování dvou hodnot
        /// </summary>
        /// <param name="n1">První hodnota</param>
        /// <param name="n2">Druhá hodnota</param>
        /// <param name="variance">Rozptyl rozdìlení</param>
        /// <param name="mean">Støední hodnota rozdìlení</param>
        private void BoxMuller(out double n1, out double n2, double variance, double mean) {
			double v1, v2, rsq;
			do {
				v1 = 2.0 * r.NextDouble() - 1.0;
				v2 = 2.0 * r.NextDouble() - 1.0;
				rsq = v1*v1 + v2*v2;			// Polomìr ^2
			} while (rsq >= 1.0 || rsq == 0.0);	// Nesmíme být vnì jednotkového kruhu nebo v 0

			double fac = System.Math.Sqrt(-2.0 * System.Math.Log(rsq) / rsq);

			n1 = v1 * fac * variance + mean;
			n2 = v2 * fac * variance + mean;
		}
	}
}
