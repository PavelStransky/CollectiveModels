using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Vypoèítá koeficienty regresní køivky daného øádu
	/// </summary>
	public class Regression {
		/// <summary>
		/// Prázdný konstruktor (abychom nemohli vytvoøit instanci)
		/// </summary>
		private Regression() {}

		/// <summary>
		/// Výpoèet regresních koeficientù
		/// </summary>
		public static Vector Compute(PointVector points, int order) {
			int dim = order + 1;

			Matrix m = new Matrix(dim);

			m[0,0] = points.Length;
			for(int i = 1; i < dim; i++) {
				double sum = 0;
				for(int k = 0; k < points.Length; k++)
					sum += System.Math.Pow(points[k].X, i);
				for(int j = 0; j <= i; j++)
					m[i - j,j] = sum;

				sum = 0;
				for(int k = 0; k < points.Length; k++)
					sum += System.Math.Pow(points[k].X, 2*dim-1-i);
				for(int j = 0; j < i; j++)
					m[dim-j-1, dim-i+j] = sum;
			}

			Vector v = new Vector(dim);
			for(int i = 0; i < dim; i++) {
				double sum = 0;
				for(int j = 0; j < points.Length; j++)
					sum += System.Math.Pow(points[j].X, i) * points[j].Y;

				v[i] = sum;
			}

			return m.Inv() * v;
		}
	}
}
