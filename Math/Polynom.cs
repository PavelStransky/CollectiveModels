using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Operace s polynomem a_{0} + a_{1}x + a_{2}x^{2} + ...
	/// kde složky a_{0}, a_{1}, ..., a_{n} tvoøí vektor
	/// </summary>
	public class Polynom {
		/// <summary>
		/// Prázdný konstruktor (abychom nemohli vytvoøit instanci)
		/// </summary>
		private Polynom() {}

		/// <summary>
		/// Hodnota polynomu v daném bodì
		/// </summary>
		/// <param name="x">Bod</param>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static double GetValue(Vector polynom, double x) {
			double result = 0;
			for(int i = 0; i < polynom.Length; i++)
				result += polynom[i] * System.Math.Pow(x, i);
			return result;
		}

		/// <summary>
		/// Vrátí hodnotu primitivní funkce (bez integraèní konstanty)
		/// </summary>
		/// <param name="x">Bod, ve kterém se poèítá</param>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static double GetIValue(Vector polynom, double x) {			
			double result = 0;
			for(int i = 0; i < polynom.Length; i++)
				result += polynom[i] * System.Math.Pow(x, i + 1) / (i + 1);
			return result;

		}

		/// <summary>
		/// Vrátí hodnotu derivace polynomu
		/// </summary>
		/// <param name="x">Bod, ve kterém se poèítá</param>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static double GetDValue(Vector polynom, double x) {			
			double result = 0;
			for(int i = 1; i < polynom.Length; i++)
				result += polynom[i] * i * System.Math.Pow(x, i - 1);
			return result;
		}

        /// <summary>
        /// Vytvoøí Hessenbergovu matici pro polynom
        /// </summary>
        /// <param name="polynom">Koeficienty polynomu</param>
        private static Matrix Hessenberg(Vector polynom) {
            // Hessenbergova matice
            Matrix hess = new Matrix(polynom.Length - 1);

            // Jmenovatel
            double d = polynom[polynom.Length - 1];

            // První øádek
            for(int i = 0; i < hess.LengthY; i++) {
                hess[0, i] = -polynom[polynom.Length - i - 2] / d;

                // Pod diagonálou 1
                if(i != 0)
                    hess[i, i - 1] = 1.0;
            }

            return hess;
        }

		/// <summary>
		/// Vyøeší rovnici P(x) == 0
		/// </summary>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static ComplexVector Solve(Vector polynom) {
			return Polynom.Hessenberg(polynom).EigenValues();
		}

        /// <summary>
        /// Vyøeší rovnici P(x) == 0 a vrátí pouze reálné koøeny
        /// </summary>
        /// <param name="polynom">Koeficienty polynomu</param>
        public static Vector SolveR(Vector polynom) {
            return Polynom.Hessenberg(polynom).RealEigenValues();
        }
    }
}
