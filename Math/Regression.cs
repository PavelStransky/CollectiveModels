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

        /// <summary>
        /// Lineární regrese vèetnì chyb koeficientù a kvadratické odchylky
        /// </summary>
        public static Vector ComputeLinear(PointVector points) {
            double sumx = 0.0;
            double sumy = 0.0;
            double sumxy = 0.0;
            double sumx2 = 0.0;
            double sumy2 = 0.0;

            int n = points.Length;

            for(int i = 0; i < n; i++) {
                PointD p = points[i];
                sumx += p.X;
                sumy += p.Y;
                sumxy += p.X * p.Y;
                sumx2 += p.X * p.X;
                sumy2 += p.Y * p.Y;
            }

            Vector v = new Vector(6);
            
            // Odhady parametrù regrese
            double a = (n * sumxy - sumx * sumy) / (n * sumx2 - sumx * sumx);
            double b = (sumy - v[1] * sumx) / n;

            v[1] = a;
            v[0] = b;

            // Odhad smìrodatné odchylky (s^2)
            v[2] = (sumy2 - v[0] * sumy - v[1] * sumxy) / (n - 2);

            // Smìrodatné odchylky parametrù
            v[3] = System.Math.Sqrt(v[2] * sumx2 / (n * sumx2 - sumx * sumx));
            v[4] = System.Math.Sqrt(n * v[2] / (n * sumx2 - sumx * sumx));

            v[5] = 0;
            for(int i = 0; i < n; i++) {
                double e = points[i].Y - (a * points[i].X + b);
                v[5] += e * e;
            }

            return v;
        }
    }
}
