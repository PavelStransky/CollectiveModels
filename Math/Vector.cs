using System;
using System.IO;
using System.Text;

namespace PavelStransky.Math {
	/// <summary>
	/// Implementace operac� s vektorem
	/// </summary>
	public class Vector : ICloneable, IExportable, ISortable {
		// Prvky vektoru
		private double [] item;

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public Vector() { }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="length">D�lka vektoru</param>
		public Vector(int length) {
			this.item = new double[length];
		}

		/// <summary>
		/// Vytvo�� vektor s referenc� na prvky
		/// </summary>
		/// <param name="item">Pole s prvky vektoru</param>
		public Vector(double [] item) {
			this.item = item;
		}
        
		/// <summary>
		/// Po�et prvk� vektoru
		/// </summary>
		public int Length {
            get {
                return this.item.Length;
            }
            set {
                double[] newItem = new double[value];

                int minLength = System.Math.Min(this.Length, value);
                for(int i = 0; i < minLength; i++)
                    newItem[i] = this.item[i];

                this.item = newItem;
            }
        }

        /// <summary>
        /// Se�te dva vektory stejn�ch rozm�r�
        /// </summary>
        public static Vector operator +(Vector v1, Vector v2) {
            if(v1.Length != v2.Length)
                throw new VectorException(errorMessageDifferentLength);

            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + v2[i];

            return result;
        }

		/// <summary>
		/// P�i�te ke ka�d� slo�ce vektoru ��slo
		/// </summary>
		public static Vector operator +(Vector v, double d) {
            int length = v.Length;
			Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = v[i] + d;

			return result;
		}

		/// <summary>
		/// Ode�te dva vektory stejn�ch rozm�r�
		/// </summary>
		public static Vector operator -(Vector v1, Vector v2) {
			if(v1.Length != v2.Length)
				throw new VectorException(errorMessageDifferentLength);

            int length = v1.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = v1[i] - v2[i];

			return result;
		}

		/// <summary>
		/// Ode�te od ka�d� slo�ky vektoru ��slo
		/// </summary>
		public static Vector operator -(Vector v, double d) {
            int length = v.Length;
            Vector result = new Vector(v.Length);

			for(int i = 0; i < length; i++)
				result[i] = v[i] - d;

			return result;
		}


		/// <summary>
		/// Ode�te od ��sla ka�dou slo�ku vektoru
		/// </summary>
		public static Vector operator -(double d, Vector v) {
            int length = v.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = d - v[i];

			return result;
		}

		/// <summary>
		/// Vyn�sob� vektor ��slem
		/// </summary>
		public static Vector operator *(Vector v, double koef) {
            int length = v.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = koef * v[i];

			return result;
        }

        /// <summary>
        /// Vyn�sob� vektor ��slem
        /// </summary>
        public static Vector operator *(double koef, Vector v) {
            return v * koef;
        }

		/// <summary>
		/// Vyd�l� vektor ��slem
		/// </summary>
		public static Vector operator /(Vector v, double koef) {
            int length = v.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = v[i] / koef;

			return result;
		}

        /// <summary>
        /// D�l� vektory prvek po prvku
        /// </summary>
        public static Vector ItemDiv(Vector v1, Vector v2) {
            if(v1.Length != v2.Length)
                throw new VectorException(errorMessageDifferentLength);

            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] / v2[i];

            return result;
        }

        /// <summary>
        /// Skal�rn� sou�in
        /// </summary>
        public static double operator *(Vector v1, Vector v2) {
			if(v1.Length != v2.Length)
				throw new VectorException(errorMessageDifferentLength);

            int length = v1.Length;
            double result = 0;

			for(int i = 0; i < length; i++)
				result += v1[i] * v2[i];

			return result;
		}

		/// <summary>
		/// N�sob� vektor matic� zleva
		/// </summary>
		public static Vector operator *(Matrix m, Vector v) {
			if(m.LengthY != v.Length)
				throw new VectorException(errorMessageMatrixVector);

            int lengthX = m.LengthX;
            int lengthY = v.Length;
            Vector result = new Vector(lengthX);

			for(int i = 0; i < lengthX; i++) {
				result[i] = 0;
                for(int j = 0; j < lengthY; j++)
                    result[i] += m[i, j] * v[j];
			}

			return result;
		}
		
		/// <summary>
		/// N�sob� vektor matic� zprava
		/// </summary>
		public static Vector operator *(Vector v, Matrix m) {
			if(m.LengthX != v.Length)
				throw new VectorException(errorMessageMatrixVector);

            int lengthX = v.Length;
            int lengthY = m.LengthY;
            Vector result = new Vector(lengthY);

			for(int i = 0; i < lengthY; i++) {
				result[i] = 0;
                for(int j = 0; j < lengthX; j++)
                    result[i] += v[j] * m[j, i];
			}

			return result;
        }

        #region Summarize
        /// <summary>
        /// Se�te vektory a vyn�sob� dan�mi koeficienty
        /// </summary>
        public static Vector Summarize(double d1, Vector v1, double d2, Vector v2) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++) 
                result[i] = d1 * v1[i] + d2 * v2[i];

            return result;
        }

        /// <summary>
        /// Se�te vektory a vyn�sob� dan�mi koeficienty
        /// </summary>
        public static Vector Summarize(Vector v1, double d2, Vector v2) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + d2 * v2[i];

            return result;
        }

        /// <summary>
        /// Se�te vektory a vyn�sob� dan�mi koeficienty
        /// </summary>
        public static Vector Summarize(double d1, Vector v1, double d2, Vector v2, double d3, Vector v3, double d4, Vector v4) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = d1 * v1[i] + d2 * v2[i] * d3 * v3[i] + d4 * v4[i];

            return result;
        }
        #endregion

        /// <summary>
        /// Oper�tor rovnosti
        /// </summary>
        public static bool operator ==(Vector v1, Vector v2) {
			if(v1.Length != v2.Length)
				return false;

			for(int i = 0; i < v1.Length; i++)
				if(v1[i] != v2[i])
					return false;

			return true;
		}

		/// <summary>
		/// Oper�tor nerovnosti
		/// </summary>
		public static bool operator !=(Vector v1, Vector v2) {
			return !(v1 == v2);
		}

		public override bool Equals(object obj) {
			return this == obj as Vector;
		}

		public override int GetHashCode() {
			double result = 0;
			for(int i = 0; i < this.Length; i++)
				result += this[i];
			return (int)(result / System.Math.Pow(10, System.Math.Ceiling(System.Math.Log10(result))) * ((double)int.MaxValue - (double)int.MinValue) + int.MinValue);
		}

		/// <summary>
		/// Speci�ln� norma vektoru (sou�et n-t�ch mocnin slo�ek)
		/// </summary>
		public double Norm(double power) {
			double result = 0;

			for(int i = 0; i < this.Length; i++)
				result += System.Math.Pow(System.Math.Abs(this[i]), power);

			return System.Math.Pow(result, 1.0 / power);
		}

		/// <summary>
		/// Eukleidovsk� norma vektoru (odmocnina sou�tu druh�ch mocnin slo�ek)
		/// </summary>
		public double EuklideanNorm() {
			return System.Math.Sqrt(this.SquaredEuklideanNorm());
		}

        /// <summary>
        /// �tverec Eukleidovsk� normy ��sti vektoru (sou�et druh�ch mocnin slo�ek)
        /// </summary>
        /// <param name="startI">Po��te�n� index pro sou�et</param>
        /// <param name="endI">Kone�n� index pro sou�et</param>
        public double SquaredEuklideanNorm(int startI, int endI) {
            double result = 0;

            for(int i = startI; i < endI; i++)
                result += this[i] * this[i];

            return result;
        }

        /// <summary>
        /// �tverec Eukleidovsk� normy vektoru (sou�et druh�ch mocnin slo�ek)
        /// </summary>
        public double SquaredEuklideanNorm() {
            return this.SquaredEuklideanNorm(0, this.Length);
        }

		/// <summary>
		/// Absolutn� norma vektoru (sou�et absolutn�ch hodnot slo�ek)
		/// </summary>
		public double AbsNorm() {
			double result = 0;

			for(int i = 0; i < this.Length; i++)
				result += System.Math.Abs(this[i]);

			return result;
		}
		
		/// <summary>
		/// Indexer
		/// </summary>
		public double this [int i] {get {return this.item[i];} set {this.item[i] = value;}}

        /// <summary>
        /// Prvn� prvek vektoru
        /// </summary>
        public double FirstItem { get { return this[0]; } set { this[0] = value; } }

        /// <summary>
        /// Posledn� prvek vektoru
        /// </summary>
        public double LastItem { get { return this[this.Length - 1]; } set { this[this.Length - 1] = value; } }

		/// <summary>
		/// Vektor vynuluje
		/// </summary>
		public void Clear() {
			for(int i = 0; i < this.Length; i++)
				this[i] = 0;
		}

		#region Implementace IExportable
		/// <summary>
		/// Ulo�� obsah vektoru do souboru
		/// </summary>
		/// <param name="export">Export</param>
        public void Export(Export export) {
            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(this.Length);
                for(int i = 0; i < this.Length; i++)
                    b.Write(this[i]);
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine(this.Length);
                for(int i = 0; i < this.Length; i++)
                    t.WriteLine(this[i]);
            }
        }

		/// <summary>
		/// Na�te obsah vektoru ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                this.item = new double[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++)
                    this[i] = b.ReadDouble();
            }
            else {
                // Textov�
                StreamReader t = import.T;
                this.item = new double[int.Parse(t.ReadLine())];
                for(int i = 0; i < this.Length; i++)
                    this[i] = double.Parse(t.ReadLine());
            }
        }
		#endregion

		/// <summary>
		/// Vytvo�� kopii vektoru
		/// </summary>
		public object Clone() {
			return new Vector((double [])this.item.Clone());
		}

		/// <summary>
		/// Transformace slo�ek vektoru podle zadan� transforma�n� funkce
		/// </summary>
		public Vector Transform(RealFunction function) {
			Vector result = new Vector(this.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = function(this[i]);

			return result;
		}

		#region Funkce Min, Max
		/// <summary>
		/// Vrac� index nejv�t��ho prvku vektoru
		/// </summary>
		public int MaxIndex() {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			int result = 0;
			double max = this[0];

			for(int i = 1; i < this.Length; i++) {
				if(max < this[i]) {
					result = i;
					max = this[i];
				}
			}

			return result;
		}

		/// <summary>
		/// Vrac� hodnotu nejv�t��ho prvku
		/// </summary>
		public double Max() {
			return this[this.MaxIndex()];
		}
		
		/// <summary>
		/// Vrac� index nejv�t��ho prvku vektoru (v absolutn� hodnot�)
		/// </summary>
		public int MaxAbsIndex() {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			int result = 0;
			double max = System.Math.Abs(this[0]);

			for(int i = 1; i < this.Length; i++) {
				double d = System.Math.Abs(this[i]);
				if(max < d) {
					result = i;
					max = d;
				}
			}

			return result;
		}

		/// <summary>
		/// Vrac� hodnotu nejv�t��ho prvku vektoru (v absolutn� hodnot�)
		/// </summary>
		public double MaxAbs() {
			return this[this.MaxAbsIndex()];
		}

		/// <summary>
		/// Vrac� index nejmen��ho prvku vektoru
		/// </summary>
		public int MinIndex() {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			int result = 0;
			double min = this[0];

			for(int i = 1; i < this.Length; i++) {
				if(min > this[i]) {
					result = i;
					min = this[i];
				}
			}

			return result;		
		}

		/// <summary>
		/// Vrac� hodnotu nejmen��ho prvku
		/// </summary>
		public double Min() {
			return this[this.MinIndex()];
		}

		/// <summary>
		/// Vrac� index nejmen��ho prvku vektoru (v absolutn� hodnot�)
		/// </summary>
		public int MinAbsIndex() {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			int result = 0;
			double min = System.Math.Abs(this[0]);

			for(int i = 1; i < this.Length; i++) {
				double d = System.Math.Abs(this[i]);
				if(min > d) {
					result = i;
					min = d;
				}
			}

			return result;		
		}

		/// <summary>
		/// Vrac� hodnotu nejmen��ho prvku vektoru (v absolutn� hodnot�)
		/// </summary>
		public double MinAbs() {
			return this[this.MinAbsIndex()];
		}
		#endregion

		#region T��d�n�
		/// <summary>
		/// Jednoduch� t��d�n� vzestupn�
		/// </summary>
		public object Sort() {
			Vector result = this.Clone() as Vector;
			Array.Sort(result.item);
			return result;
		}

		/// <summary>
		/// Jednoduch� t��d�n� sestupn�
		/// </summary>
        public object SortDesc() {
			Vector result = this.Sort() as Vector;
			Array.Reverse(result.item);
			return result;
		}

		/// <summary>
		/// Pou�ije kl�� k set��d�n� vektoru
		/// </summary>
		/// <param name="keys">Kl��e k set��d�n�</param>
        public object Sort(Vector keys) {
			Array result = this.item.Clone() as Array;
			Array kv = keys.item.Clone() as Array;
			Array.Sort(kv, result);
			return new Vector(result as double []);
		}

		/// <summary>
		/// Pou�ije kl�� k set��d�n� vektoru sestupn�
		/// </summary>
		/// <param name="keys">Kl��e k set��d�n�</param>
        public object SortDesc(Vector keys) {
			Vector result = this.Sort(keys) as Vector;
			Array.Reverse(result.item);
			return result;
		}

		/// <summary>
		/// Pou�ije vektor jako kl�� k set��d�n� objekt�
		/// </summary>
		/// <param name="toSort">Objekty k set��d�n�</param>
		public Array Sort(Array toSort) {
			Array result = toSort.Clone() as Array;
			Vector keysResult = this.Clone() as Vector;

			Array.Sort(keysResult.item, result);
			return result;
		}
		
		/// <summary>
		/// Pou�ije vektor jako kl�� k set��d�n� objekt� sestupn�
		/// </summary>
		/// <param name="toSort">Objekty k set��d�n�</param>
		public Array SortDesc(Array toSort) {
			Array result = toSort.Clone() as Array;
			Vector keysResult = this.Clone() as Vector;

			Array.Sort(keysResult.item, result);
			Array.Reverse(result);
			return result;
		}		
		#endregion

		/// <summary>
		/// St�edn� hodnota mezi indexy iStart, iEnd
		/// </summary>
		/// <param name="iStart">Po��te�n� index v�po�tu</param>
		/// <param name="iEnd">Koncov� index v�po�tu</param>
		public double PartialMean(int iStart, int iEnd) {
			double result = 0;

			for(int i = iStart; i < iEnd; i++)
				result += this[i];

			return result / (iEnd - iStart);
		}

		/// <summary>
		/// Rozptyl ��sti vektoru mezi indexy iStart, iEnd
		/// </summary>
		/// <param name="iStart">Po��te�n� index v�po�tu</param>
		/// <param name="iEnd">Koncov� index v�po�tu</param>
		public double PartialVariance(int iStart, int iEnd) {
			double result = 0;
			double mean = this.PartialMean(iStart, iEnd);

			for(int i = iStart; i < iEnd; i++) {
				double d = this[i] - mean;
				result += d*d;
			}

			return System.Math.Sqrt(result / (iEnd - iStart));
		}

		/// <summary>
		/// St�edn� hodnota
		/// </summary>
		public double Mean () {
			double result = 0;

			for(int i = 0; i < this.Length; i++)
				result += this[i];

			return result / this.Length;
		}
		
		/// <summary>
		/// Rozptyl
		/// </summary>
		public double SquaredVariance () {
			double result = 0;
			double mean = this.Mean();

			for(int i = 0; i < this.Length; i++) {
				double d = this[i] - mean;
				result += d*d;
			}

			return result / this.Length;
		}

		public double Variance () {
			return System.Math.Sqrt(this.SquaredVariance());
		}

		/// <summary>
		/// Vr�t� histogram n�hodn�ho vektoru
		/// </summary>
		/// <param name="intervals">Po�et interval�</param>
		/// <param name="min">Po��te�n� hodnota, od kter� se histogram po��t�</param>
		/// <param name="max">Maxim�ln� hodnota, do kter� se histogram po��t�</param>
		public PointVector Histogram(int intervals, double min, double max) {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			PointVector result = new PointVector(intervals);
			Vector sorted = this.Sort() as Vector;

			double step = (max - min) / intervals;

			int j = 0;
			for(int i = 0; i < intervals; i++) {
				result[i].X = min + step*((double)i + 0.5);
				result[i].Y = 0;
				while((sorted[j] <= min + step*(i + 1)) && (j < sorted.Length - 1)) {
					result[i].Y++;
					j++;
				}
			}

			return result;
		}

		/// <summary>
		/// Vr�t� hustotu n�hodn�ho vektoru
		/// </summary>
		/// <param name="intervals">Po�et interval�</param>
		/// <param name="minX">Po��te�n� bod intervalu, od kter�ho se hustota po��t�</param>
		/// <param name="maxX">Kone�n� bod intervalu, do kter�ho se hustota po��t�</param>
		public PointVector Density(int intervals, double minX, double maxX) {
			PointVector result = this.Histogram(intervals, minX, maxX);
			return result * new PointD(1, 1.0 / result.Integrate());
		}

		/// <summary>
		/// Vr�t� odhad kumulovan�ho histogramu n�hodn�ho vektoru
		/// </summary>
		/// <param name="intervals">Po�et interval�</param>
		/// <param name="min">Po��te�n� hodnota, od kter� se histogram po��t�</param>
		/// <param name="max">Maxim�ln� hodnota, do kter� se histogram po��t�</param>
		public PointVector CumulativeHistogram(int intervals, double min, double max) {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			PointVector result = new PointVector(intervals);
			Vector sorted = this.Sort() as Vector;

			double step = (max - min) / intervals;

			int j = 0;
			for(int i = 0; i < intervals; i++) {
				result[i].X = min + step*((double)i + 0.5);
				result[i].Y = i == 0 ? 0 : result[i - 1].Y;
				while((sorted[j] <= min + step * (i + 1)) && (j < sorted.Length - 1)) {
					result[i].Y++;
					j++;
				}
			}

			return result;
		}

		/// <summary>
		/// Vr�t� odhad distribu�n� funkce n�hodn�ho vektoru
		/// </summary>
		/// <param name="intervals">Po�et interval�</param>
		/// <param name="minX">Po��te�n� bod intervalu, od kter�ho se distribu�n� funkce po��t�</param>
		/// <param name="maxX">Kone�n� bod intervalu, do kter�ho se distribu�n� funkce po��t�</param>
		public PointVector DistributionFunction(int intervals, double minX, double maxX) {
			PointVector result = this.CumulativeHistogram(intervals, minX, maxX);
			return result * new PointD(1, 1.0 / result[intervals].Y);
		}

		/// <summary>
		/// Normalizuje dan� vektor podle euklidovsk� normy
		/// </summary>
		public Vector EuklideanNormalization() {
			Vector result;
			result = this / this.EuklideanNorm();

			return result;
		}

		/// <summary>
		/// Normalizuje dan� vektor podle absolutn� normy
		/// </summary>
		public Vector AbsNormalization() {
			Vector result;
			result = this / this.AbsNorm();

			return result;
		}

		/// <summary>
		/// Normalizuje dan� vektor tak, aby m�l nulov� pr�m�r jednotkov� rozptyl
		/// </summary>
		public Vector StatisticalNormalization() {
			Vector result;
			result = (this - this.Mean()) / this.Variance();

			return result;
		}

        /// <summary>
        /// Absolutn� hodnota vektoru
        /// </summary>
        public Vector Abs() {
            Vector result = new Vector(this.Length);

            for(int i = 0; i < result.Length; i++)
                result[i] = System.Math.Abs(this[i]);

            return result;
        }

        /// <summary>
        /// Spoj� dva vektory
        /// </summary>
        public static Vector Join(Vector v1, Vector v2) {
            int numItems = v1.Length + v2.Length;
            Vector result = new Vector(numItems);

            for(int i = 0; i < v1.Length; i++)
                result[i] = v1[i];

            for(int i = 0; i < v2.Length; i++)
                result[i + v1.Length] = v2[i];

            return result;
        }

        /// <summary>
        /// Spoj� vektory do jednoho
        /// </summary>
        /// <param name="vArray">�ada vektor�</param>
        public static Vector Join(Vector[] vArray) {
			// Po�et prvk� v�sledn�ho vektoru
			int numItems = 0;
			for(int i = 0; i < vArray.Length; i++)
				numItems += vArray[i].Length;
			
			Vector result = new Vector(numItems);

			int item = 0;
			for(int i = 0; i < vArray.Length; i++)
				for(int j = 0; j < vArray[i].Length; j++)
					result[item++] = vArray[i][j];

			return result;
		}

		/// <summary>
		/// Vypo��t� number of principal components
		/// </summary>
		public double NumPrincipalComponents() {
			double result = 0;

			int length = this.Length;
			for(int i = 0; i < length; i++) {
				double pow2 = this[i] * this[i];
				result += pow2 * pow2;
			}
			result *= length;

			return 1.0 / result;
		}

		/// <summary>
		/// Vypo��t� Symmetry parameter vektoru
		/// </summary>
		public double Symmetry() {
			double result = 0;
			
			int length = this.Length;
			for(int i = 0; i < length; i++)
				result += this[i] * System.Math.Abs(this[i]);

			return System.Math.Abs(result);
		}

		/// <summary>
		/// P�evede vektor na �et�zec
		/// </summary>
		public override string ToString() {
			StringBuilder s = new StringBuilder();

			for(int i = 0; i < this.Length; i++) 
				s.Append(string.Format("{0,10:#####0.000}", this[i]));

			return s.ToString();
		}

		private const string errorMessageNoData = "K proveden� operace je nutn�, aby d�lka vektoru nebyla nulov�.";
		private const string errorMessageDifferentLength = "K proveden� operace mus� m�t vektory stejnou d�lku.";
		private const string errorMessageMatrixVector = "P�i n�soben� matice a vektoru mus� m�t oba objekty kompatibiln� rozm�ry.";
	}

	/// <summary>
	/// V�jimka ve t��d� Vector
	/// </summary>
	public class VectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public VectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public VectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve t��d� Vector do�lo k chyb�: ";
	}
}
