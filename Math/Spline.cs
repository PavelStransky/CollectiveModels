using System;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// Zadanými hodnotami proloží kubický spline
	/// </summary>
	public class Spline: IExportable {
		// Vstupní data
		private PointVector data;
		// Vektor druhých derivací
		private Vector d2;

		public Spline(PointVector data) {
			if(data.Length <= 2)
				throw new Exception(Messages.EMFewPointsSpline);

			this.data = data.SortX();
			this.d2 = this.ComputeDerivation();
		}

		private Vector ComputeDerivation() {
			Matrix m = new Matrix(4, this.data.Length);
			Vector result;
		
			double xn = this.data[1].X - this.data[0].X;
			double yn = this.data[1].Y - this.data[0].Y;

			for(int i = 1; i < this.data.Length - 1; i++) {
				double xo = xn;
				xn = this.data[i + 1].X - this.data[i].X;
		
				double yo = yn;
				yn = this.data[i + 1].Y - this.data[i].Y;
		
				m[0, i] = yn / xn - yo / xo;
				m[1, i] = xo / 6.0;
				m[2, i] = (xo + xn) / 3.0;
				m[3, i] = xn / 6.0;
			}
	
			int n = this.data.Length - 2;
			for(int i = 2; i <= n; i++) {
				double d = m[1, i] / m[2, i - 1];
				m[2, i] -= m[3, i - 1] * d;
				m[1, i] = 0;
		
				m[0, i] -= m[0, i - 1] * d;
			}
	
			m[0, n] /= m[2, n];
	
			for(int i = n - 1; i >= 1; i--)
				m[0, i] = (m[0, i] - m[3, i] * m[0, i + 1]) / m[2, i];
			 
			result = m.GetRowVector(0);

            result[0] = 0;
            result[result.Length - 1] = 0;

			return result;
		}

		/// <summary>
		/// Indexer (vrací aproximovanou hodnotu v daném bodì)
		/// </summary>
		public double this[double x] {
			get {
				double t, A, B, C, D, dx;
	
                if(x < this.data.FirstItem.X || x > this.data.LastItem.X)
                    return 0;

                int length = this.data.Length;
                int i = this.FindIndex(x);
	
				dx = (this.data[i].X - this.data[i - 1].X);
				t = (x - this.data[i - 1].X) / dx;

				A = 1 - t;
				B = t;
				C = (A * A * A - A) * dx * dx / 6.0;
				D = (B * B * B - B) * dx * dx / 6.0;
	
				return A * this.data[i - 1].Y + B * this.data[i].Y + C * this.d2[i - 1] + D * this.d2[i];
			}
		}

        /// <summary>
        /// Najde index prvku, pro který platí data[i - 1] less x lessequal data[i]
        /// </summary>
        /// <param name="x">Bod x</param>
        private int FindIndex(double x) {
            int maxi = this.data.Length;
            int mini = 0;

            while(maxi - 1 != mini) {
                int i = (maxi + mini) / 2;
                if(this.data[i].X <= x)
                    mini = i;
                else
                    maxi = i;
            }
            if(maxi >= this.data.Length)
                maxi--;

            return maxi;
        }

		/// <summary>
		/// Vrací aproximovanou hodnotu
		/// </summary>
		/// <param name="x">Bod</param>
		public double GetValue(double x) {
			return this[x];
		}

        public Vector GetValue(Vector x) {
            int length = x.Length;
            int count = data.Length;
            Vector result = new Vector(length);

            int i = 0;

            double d = this.data.FirstItem.X;
            while(x[i] < d)
                result[i++] = 0.0;

            int j = 0;
            for(; i < length; i++) {
                d = x[i];

                while(j < count && d > this.data[j].X)
                    j++;

                if(j >= count)
                    break;

                double dx = (this.data[j].X - this.data[j - 1].X);
                double t = (d - this.data[j - 1].X) / dx;

                double A = 1 - t;
                double B = t;
                double C = (A * A * A - A) * dx * dx / 6.0;
                double D = (B * B * B - B) * dx * dx / 6.0;

                result[i] = A * this.data[j - 1].Y + B * this.data[j].Y + C * this.d2[j - 1] + D * this.d2[j];
            }

            return result;
        }

        /// <summary>
        /// Vrátí vektor apoximovaných dat v zadaném rozmezí startX - EndX
        /// </summary>
        /// <param name="number">Poèet bodù ve výstupním vektoru</param>
        /// <param name="startX">Poèáteèní hodnota nezávisle promìnné</param>
        /// <param name="endX">Koncová hodnota nezávisle promìnné</param>
        public PointVector GetPointVector(int number, double startX, double endX) {
			PointVector result = new PointVector(number);

			double koef = (endX - startX) / number;

			for(int i = 0; i < result.Length; i++) {
				double x = koef * i + startX;
				result[i].X = x;
				result[i].Y = this[x];
			}

			return result;
		}

		/// <summary>
		/// Vrátí vektor aproximovaných dat v rozmezí mezi minimální a maximální hodnotou vstupních dat
		/// </summary>
		/// <param name="number">Poèet bodù ve výstupním vektoru</param>
		public PointVector GetPointVector(int number) {
			return this.GetPointVector(number, this.data.MinX(), this.data.MaxX());
		}

        #region IExportable Members
        /// <summary>
        /// Export
        /// </summary>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.data);
            param.Export(export);
        }

        /// <summary>
        /// Import
        /// </summary>
        public Spline(Core.Import import) {
            IEParam param = new IEParam(import);
            this.data = param.Get() as PointVector;
            this.ComputeDerivation();
        }
        #endregion
    }
}
