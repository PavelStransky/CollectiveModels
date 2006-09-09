using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Operace s polynomem a_{0} + a_{1}x + a_{2}x^{2} + ...
	/// kde slo�ky a_{0}, a_{1}, ..., a_{n} tvo�� vektor
	/// </summary>
	public class Polynom {
		/// <summary>
		/// Pr�zdn� konstruktor (abychom nemohli vytvo�it instanci)
		/// </summary>
		private Polynom() {}

		/// <summary>
		/// Hodnota polynomu v dan�m bod�
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
		/// Vr�t� hodnotu primitivn� funkce (bez integra�n� konstanty)
		/// </summary>
		/// <param name="x">Bod, ve kter�m se po��t�</param>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static double GetIValue(Vector polynom, double x) {			
			double result = 0;
			for(int i = 0; i < polynom.Length; i++)
				result += polynom[i] * System.Math.Pow(x, i + 1) / (i + 1);
			return result;

		}

		/// <summary>
		/// Vr�t� hodnotu derivace polynomu
		/// </summary>
		/// <param name="x">Bod, ve kter�m se po��t�</param>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static double GetDValue(Vector polynom, double x) {			
			double result = 0;
			for(int i = 1; i < polynom.Length; i++)
				result += polynom[i] * i * System.Math.Pow(x, i - 1);
			return result;
		}

        /// <summary>
        /// Vytvo�� Hessenbergovu matici pro polynom
        /// </summary>
        /// <param name="polynom">Koeficienty polynomu</param>
        private static Matrix Hessenberg(Vector polynom) {
            // Hessenbergova matice
            Matrix hess = new Matrix(polynom.Length - 1);

            // Jmenovatel
            double d = polynom[polynom.Length - 1];

            // Prvn� ��dek
            for(int i = 0; i < hess.LengthY; i++) {
                hess[0, i] = -polynom[polynom.Length - i - 2] / d;

                // Pod diagon�lou 1
                if(i != 0)
                    hess[i, i - 1] = 1.0;
            }

            return hess;
        }

		/// <summary>
		/// Vy�e�� rovnici P(x) == 0
		/// </summary>
		/// <param name="polynom">Koeficienty polynomu</param>
		public static ComplexVector Solve(Vector polynom) {
			return Polynom.Hessenberg(polynom).EigenValues();
		}

        /// <summary>
        /// Vy�e�� rovnici P(x) == 0 a vr�t� pouze re�ln� ko�eny
        /// </summary>
        /// <param name="polynom">Koeficienty polynomu</param>
        public static Vector SolveR(Vector polynom) {
            return Polynom.Hessenberg(polynom).RealEigenValues();
        }
    }
}
