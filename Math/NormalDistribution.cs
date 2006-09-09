using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Tøída generující Gaussovo náhodné rozdìlení
	/// f(x) = 1 / (sqrt(2 pi) sigma) exp(-(x - mi)^2 / (2 sigma^2))
	/// </summary>
	public class NormalDistribution {
		private double mean;
		private double variance;

		/// <summary>
		/// Generátor rovnomìrnì rozdìlených náhodných èísel
		/// </summary>
		private Random r;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="mean">Støední hodnota rozdìlení</param>
		/// <param name="variance">Rozptyl rozdìlení</param>
		public NormalDistribution(double mean, double variance) {
			this.mean = mean;
			this.variance = variance;
			this.r = new Random();
		}

		/// <summary>
		/// Vrátí jednu hodnotu
		/// </summary>
		public double GetValue() {
			double n1, n2;
			this.BoxMuller(out n1, out n2);

			return n1;
		}

		/// <summary>
		/// Vrátí vektor
		/// </summary>
		/// <param name="length">Délka vektoru</param>
		public Vector GetVector(int length) {
			double n1, n2;
			Vector result = new Vector(length);

			for(int i = 0; i < length / 2; i++) {
				this.BoxMuller(out n1, out n2);
				result[2*i] = n1;
				result[2*i + 1] = n2;
			}

			// Pokud je poèet prvkù lichý, vyplníme ještì poslední hodnotu
			if(length % 2 > 0) {
				this.BoxMuller(out n1, out n2);
				result[length - 1] = n1;
			}

			return result;
		}

		/// <summary>
		/// Box - Mullerova metoda generování dvou hodnot
		/// </summary>
		/// <param name="n1">První hodnota</param>
		/// <param name="n2">Druhá hodnota</param>
		private void BoxMuller(out double n1, out double n2) {
			double v1, v2, rsq;
			do {
				v1 = 2.0 * r.NextDouble() - 1.0;
				v2 = 2.0 * r.NextDouble() - 1.0;
				rsq = v1*v1 + v2*v2;			// Polomìr ^2
			} while (rsq >= 1.0 || rsq == 0.0);	// Nesmíme být vnì jednotkového kruhu nebo v 0

			double fac = System.Math.Sqrt(-2.0 * System.Math.Log(rsq) / rsq);

			n1 = v1 * fac * this.variance + this.mean;
			n2 = v2 * fac * this.variance + this.mean;
		}
	}
}
