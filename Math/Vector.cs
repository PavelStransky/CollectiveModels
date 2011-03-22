using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// Implementace operací s vektorem
	/// </summary>
	public class Vector : ICloneable, IExportable, ISortable {
        public enum HistogramTypes {
            Point,
            Line,
            Bar
        }

		// Prvky vektoru
		private double [] item;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="length">Délka vektoru</param>
		public Vector(int length) {
			this.item = new double[length];
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="item">Pole s prvky vektoru</param>
        public Vector(double[] item) {
            this.item = (double[])item.Clone();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="item">Pole s prvky vektoru</param>
        public Vector(int[] item) {
            int length = item.Length;

            this.item = new double[length];
            for(int i = 0; i < length; i++)
                this[i] = item[i];
        }

        /// <summary>
        /// Poèet prvkù vektoru
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
        /// Hledá daný prvek ve vektoru
        /// </summary>
        /// <param name="d">Hledané èíslo</param>
        /// <returns>Nezáporné èíslo, pokud byl prvek nalezen, -1 v opaèném pøípadì.</returns>
        public int Find(double d) {
            int length = this.Length;

            for(int i = 0; i < length; i++)
                if(this[i] == d)
                    return i;

            return -1;
        }

        /// <summary>
        /// Seète dva vektory stejných rozmìrù
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
		/// Pøiète ke každé složce vektoru èíslo
		/// </summary>
		public static Vector operator +(Vector v, double d) {
            int length = v.Length;
			Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = v[i] + d;

			return result;
		}

		/// <summary>
		/// Odeète dva vektory stejných rozmìrù
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
		/// Odeète od každé složky vektoru èíslo
		/// </summary>
		public static Vector operator -(Vector v, double d) {
            int length = v.Length;
            Vector result = new Vector(v.Length);

			for(int i = 0; i < length; i++)
				result[i] = v[i] - d;

			return result;
		}


		/// <summary>
		/// Odeète od èísla každou složku vektoru
		/// </summary>
		public static Vector operator -(double d, Vector v) {
            int length = v.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = d - v[i];

			return result;
		}

		/// <summary>
		/// Vynásobí vektor èíslem
		/// </summary>
		public static Vector operator *(Vector v, double koef) {
            int length = v.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = koef * v[i];

			return result;
        }

        /// <summary>
        /// Vynásobí vektor èíslem
        /// </summary>
        public static Vector operator *(double koef, Vector v) {
            return v * koef;
        }

		/// <summary>
		/// Vydìlí vektor èíslem
		/// </summary>
		public static Vector operator /(Vector v, double koef) {
            int length = v.Length;
            Vector result = new Vector(length);

			for(int i = 0; i < length; i++)
				result[i] = v[i] / koef;

			return result;
		}

        /// <summary>
        /// Dìlí vektory prvek po prvku
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
        /// Násobí vektory prvek po prvku
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static object ItemMul(Vector v1, Vector v2) {
            if(v1.Length != v2.Length)
                throw new VectorException(errorMessageDifferentLength);

            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] * v2[i];

            return result;
        }

        /// <summary>
        /// Skalární souèin
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
		/// Násobí vektor maticí zleva
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
		/// Násobí vektor maticí zprava
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
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(double d1, Vector v1, double d2, Vector v2) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++) 
                result[i] = d1 * v1[i] + d2 * v2[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(Vector v1, double d2, Vector v2) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + d2 * v2[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(Vector v1, double d2, Vector v2, double d3, Vector v3) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + d2 * v2[i] + d3 * v3[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(Vector v1, double d2, Vector v2, double d3, Vector v3, double d4, Vector v4) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + d2 * v2[i] + d3 * v3[i] + d4 * v4[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(Vector v1, double d2, Vector v2, double d3, Vector v3, double d4, Vector v4, double d5, Vector v5) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + d2 * v2[i] + d3 * v3[i] + d4 * v4[i] + d5 * v5[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(Vector v1, double d2, Vector v2, double d3, Vector v3, double d4, Vector v4, double d5, Vector v5, double d6, Vector v6) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = v1[i] + d2 * v2[i] + d3 * v3[i] + d4 * v4[i] + d5 * v5[i] + d6 * v6[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(double d1, Vector v1, double d2, Vector v2, double d3, Vector v3, double d4, Vector v4) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = d1 * v1[i] + d2 * v2[i] + d3 * v3[i] + d4 * v4[i];

            return result;
        }

        /// <summary>
        /// Seète vektory a vynásobí danými koeficienty
        /// </summary>
        public static Vector Summarize(double d1, Vector v1, double d2, Vector v2, double d3, Vector v3, double d4, Vector v4, double d5, Vector v5) {
            int length = v1.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = d1 * v1[i] + d2 * v2[i] + d3 * v3[i] + d4 * v4[i] + d5 * v5[i];

            return result;
        }
        #endregion

		public override bool Equals(object obj) {
            Vector v1 = this;
            Vector v2 = obj as Vector;

			if(v1.Length != v2.Length)
				return false;

			for(int i = 0; i < v1.Length; i++)
				if(v1[i] != v2[i])
					return false;

			return true;
		}

		public override int GetHashCode() {
			double result = 0;
			for(int i = 0; i < this.Length; i++)
				result += this[i];
			return (int)(result / System.Math.Pow(10, System.Math.Ceiling(System.Math.Log10(result))) * ((double)int.MaxValue - (double)int.MinValue) + int.MinValue);
		}

		/// <summary>
		/// Speciální norma vektoru (souèet n-tých mocnin složek)
		/// </summary>
		public double Norm(double power) {
			double result = 0;

			for(int i = 0; i < this.Length; i++)
				result += System.Math.Pow(System.Math.Abs(this[i]), power);

			return System.Math.Pow(result, 1.0 / power);
		}

		/// <summary>
		/// Eukleidovská norma vektoru (odmocnina souètu druhých mocnin složek)
		/// </summary>
		public double EuklideanNorm() {
			return System.Math.Sqrt(this.SquaredEuklideanNorm());
		}

        /// <summary>
        /// Ètverec Eukleidovské normy èásti vektoru (souèet druhých mocnin složek)
        /// </summary>
        /// <param name="startI">Poèáteèní index pro souèet</param>
        /// <param name="endI">Koneèný index pro souèet</param>
        public double SquaredEuklideanNorm(int startI, int endI) {
            double result = 0;

            for(int i = startI; i < endI; i++)
                result += this[i] * this[i];

            return result;
        }

        /// <summary>
        /// Ètverec Eukleidovské normy vektoru (souèet druhých mocnin složek)
        /// </summary>
        public double SquaredEuklideanNorm() {
            return this.SquaredEuklideanNorm(0, this.Length);
        }

		/// <summary>
		/// Absolutní norma vektoru (souèet absolutních hodnot složek)
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
        /// Indexer pro více prvkù
        /// </summary>
        //public Vector this[params int[] index] {
        //    get {
        //        int length = index.Length;
        //        Vector result = new Vector(length);
        //        for(int i = 0; i < length; i++)
        //            result[i] = this[index[i]];
        //        return result;
        //    }
        //}

        /// <summary>
        /// První prvek vektoru
        /// </summary>
        public double FirstItem { get { return this[0]; } set { this[0] = value; } }

        /// <summary>
        /// Poslední prvek vektoru
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
		/// Uloží obsah vektoru do souboru
		/// </summary>
		/// <param name="export">Export</param>
        public void Export(Export export) {
            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;
                b.Write(this.Length);
                for(int i = 0; i < this.Length; i++)
                    b.Write(this[i]);
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine(this.Length);
                for(int i = 0; i < this.Length; i++)
                    t.WriteLine(this[i]);
            }
        }

		/// <summary>
		/// Naète obsah vektoru ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public Vector(Core.Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                this.item = new double[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++)
                    this[i] = b.ReadDouble();
            }
            else {
                // Textovì
                StreamReader t = import.T;
                this.item = new double[int.Parse(t.ReadLine())];
                for(int i = 0; i < this.Length; i++)
                    this[i] = double.Parse(t.ReadLine());
            }
        }
		#endregion

		/// <summary>
		/// Vytvoøí kopii vektoru
		/// </summary>
		public object Clone() {
			return new Vector((double [])this.item.Clone());
		}

		/// <summary>
		/// Transformace složek vektoru podle zadané transformaèní funkce
		/// </summary>
		public Vector Transform(RealFunction function) {
			Vector result = new Vector(this.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = function(this[i]);

			return result;
		}

		/// <summary>
		/// Transformace složek vektoru podle zadané transformaèní funkce
		/// </summary>
        public Vector Transform(RealFunctionWithParams function, params object[] p) {
            Vector result = new Vector(this.Length);

            for(int i = 0; i < result.Length; i++)
                result[i] = function(this[i], p);

            return result;
        }

        /// <summary>
        /// Vrátí všechny indexy, které splòují podmínku danou hodnotou a operátorem porovnávání
        /// </summary>
        /// <param name="x">Hodnota pro porovnávání</param>
        /// <param name="o">Operátor</param>
        public int[] GetIndex(double x, ComparisonOperator o) {
            ArrayList index = new ArrayList();

            int length = this.Length;
            for(int i = 0; i < length; i++)
                if(o.Compare(this[i], x))
                    index.Add(i);

            int[] result = new int[index.Count];
            int j = 0;
            foreach(int i in index)
                result[j++] = i;

            return result;
        }

        #region Funkce Min, Max
        /// <summary>
        /// Vrací index nejvìtšího prvku vektoru
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
		/// Vrací hodnotu nejvìtšího prvku
		/// </summary>
		public double Max() {
			return this[this.MaxIndex()];
		}
		
		/// <summary>
		/// Vrací index nejvìtšího prvku vektoru (v absolutní hodnotì)
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
		/// Vrací hodnotu nejvìtšího prvku vektoru (v absolutní hodnotì)
		/// </summary>
		public double MaxAbs() {
			return this[this.MaxAbsIndex()];
		}

		/// <summary>
		/// Vrací index nejmenšího prvku vektoru
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
		/// Vrací hodnotu nejmenšího prvku
		/// </summary>
		public double Min() {
			return this[this.MinIndex()];
		}

		/// <summary>
		/// Vrací index nejmenšího prvku vektoru (v absolutní hodnotì)
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
		/// Vrací hodnotu nejmenšího prvku vektoru (v absolutní hodnotì)
		/// </summary>
		public double MinAbs() {
			return this[this.MinAbsIndex()];
		}
		#endregion

		#region Tøídìní
        /// <summary>
        /// Keys for sorting
        /// </summary>
        public Array GetKeys() {
            return this.item.Clone() as Array;
        }

		/// <summary>
		/// Jednoduché tøídìní vzestupnì
		/// </summary>
		public object Sort() {
			Vector result = this.Clone() as Vector;
			Array.Sort(result.item);
			return result;
		}

		/// <summary>
		/// Jednoduché tøídìní sestupnì
		/// </summary>
        public object SortDesc() {
			Vector result = this.Sort() as Vector;
			Array.Reverse(result.item);
			return result;
		}

		/// <summary>
		/// Použije klíè k setøídìní vektoru
		/// </summary>
		/// <param name="keys">Klíèe k setøídìní</param>
        public object Sort(ISortable keys) {
			Vector result = this.Clone() as Vector;
			Array kv = keys.GetKeys();
			Array.Sort(kv, result.item);
			return result;
		}

		/// <summary>
		/// Použije klíè k setøídìní vektoru sestupnì
		/// </summary>
		/// <param name="keys">Klíèe k setøídìní</param>
        public object SortDesc(ISortable keys) {
			Vector result = this.Sort(keys) as Vector;
			Array.Reverse(result.item);
			return result;
		}
        #endregion

        /// <summary>
        /// Odstraní složky vektoru, které se vyskytují cícekrát
        /// </summary>
        public Vector RemoveDuplicity() {
            Vector result = (Vector)this.Clone();
            int length = this.Length;

            int p = 0;

            for(int i = 0; i < length; i++) {
                double d = this[i];
                bool found = false;
                for(int j = 0; j < p; j++)
                    if(result[j] == d) {
                        found = true;
                        break;
                    }
                if(!found) 
                    result[p++] = d;
            }

            result.Length = p;

            return result;
        }

        /// <summary>
        /// Souèet prvkù vektoru mezy indexy iStart, iEnd
        /// </summary>
        /// <param name="iStart">Poèáteèní index výpoètu</param>
        /// <param name="iEnd">Koncový index výpoètu</param>
        public double Sum(int iStart, int iEnd) {
            double result = 0;

            for(int i = iStart; i < iEnd; i++)
                result += this[i];

            return result;
        }

        /// <summary>
        /// Souèet prvkù vektoru v absolutní hodnotì
        /// </summary>
        public double SumAbs(int iStart, int iEnd) {
            double result = 0;

            for(int i = iStart; i < iEnd; i++)
                result += System.Math.Abs(this[i]);

            return result;
        }

		/// <summary>
		/// Støední hodnota mezi indexy iStart, iEnd
		/// </summary>
		/// <param name="iStart">Poèáteèní index výpoètu</param>
		/// <param name="iEnd">Koncový index výpoètu</param>
		public double Mean(int iStart, int iEnd) {
			return this.Sum(iStart, iEnd) / (iEnd - iStart);
		}

		/// <summary>
		/// Rozptyl èásti vektoru mezi indexy iStart, iEnd
		/// </summary>
		/// <param name="iStart">Poèáteèní index výpoètu</param>
		/// <param name="iEnd">Koncový index výpoètu</param>
		public double Variance(int iStart, int iEnd) {
			double result = 0;
			double mean = this.Mean(iStart, iEnd);

			for(int i = iStart; i < iEnd; i++) {
				double d = this[i] - mean;
				result += d*d;
			}

			return System.Math.Sqrt(result / (iEnd - iStart));
		}

        /// <summary>
        /// Souèet prvkù vektoru
        /// </summary>
        public double Sum() {
            int length = this.Length;
            double result = 0;

            for(int i = 0; i < length; i++)
                result += this[i];

            return result;
        }

        /// <summary>
        /// Souèet prvkù vektoru v absolutní hodnotì
        /// </summary>
        public double SumAbs() {
            int length = this.Length;
            double result = 0;

            for(int i = 0; i < length; i++)
                result += System.Math.Abs(this[i]);

            return result;
        }

        /// <summary>
        /// Støední hodnota
        /// </summary>
        public double Mean() {
			return this.Sum() / this.Length;
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
        /// Vrátí histogram vektoru
        /// </summary>
        /// <param name="intervals">Poèet intervalù</param>
        /// <param name="min">Poèáteèní hodnota, od které se histogram poèítá</param>
        /// <param name="max">Maximální hodnota, do které se histogram poèítá</param>
        public PointVector Histogram(int intervals, double min, double max) {
            return this.Histogram(intervals, min, max, HistogramTypes.Point);
        }

		/// <summary>
		/// Vrátí histogram vektoru
		/// </summary>
		/// <param name="intervals">Poèet intervalù</param>
		/// <param name="min">Poèáteèní hodnota, od které se histogram poèítá</param>
		/// <param name="max">Maximální hodnota, do které se histogram poèítá</param>
        /// <param name="type">Typ histogramu</param>
		public PointVector Histogram(int intervals, double min, double max, HistogramTypes type) {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

            int lengthR = this.HistogramLength(intervals, type);

			PointVector result = new PointVector(lengthR);
			Vector sorted = this.Sort() as Vector;

			double step = (max - min) / intervals;
            double minx = min;

			int j = 0;
			for(int i = 0; i < intervals; i++) {
                double maxx = min + step * (i + 1);
                double y = 0;
				while((j < sorted.Length) && (sorted[j] <= maxx)) {
                    y++;
					j++;
				}

                this.FillHistogramResult(result, i, minx, maxx, y, type);
                minx = maxx;
			}

			return result;
		}

        /// <summary>
        /// Vrátí histogram vektoru
        /// </summary>
        /// <param name="interval">Points of the interval</param>
        public PointVector Histogram(Vector interval) {
            return this.Histogram(interval, HistogramTypes.Point);
        }

        /// <summary>
        /// Vrátí histogram vektoru
        /// </summary>
        /// <param name="interval">Points of the interval</param>
        /// <param name="type">Typ histogramu</param>
        public PointVector Histogram(Vector interval, HistogramTypes type) {
            if(this.Length == 0)
                throw new VectorException(errorMessageNoData); 
            
            int length = this.Length;
            int lengthR = this.HistogramLength(interval.Length - 1, type);

            PointVector result = new PointVector(lengthR);

            for(int i = 0; i < interval.Length - 1; i++) {
                double minx = interval[i];
                double maxx = interval[i + 1];
                int y = 0;

                for(int j = 0; j < length; j++) {
                    double x = this[j];

                    if(x >= minx && x < maxx)
                        y++;
                }

                this.FillHistogramResult(result, i, minx, maxx, y, type);
            }

            return result;
        }

        /// <summary>
        /// Poèet bodù histogramu
        /// </summary>
        /// <param name="length">Poèet intervalù histogramu</param>
        /// <param name="type">Typ histogramu</param>
        private int HistogramLength(int length, HistogramTypes type) {
            if(type == HistogramTypes.Point)
                return length;
            else if(type == HistogramTypes.Line)
                return 2 * length;
            else if(type == HistogramTypes.Bar)
                return 4 * length;

            return 0;
        }

        /// <summary>
        /// Vyplní jeden bod do histogramu
        /// </summary>
        private void FillHistogramResult(PointVector result, int i, double minx, double maxx, double y, HistogramTypes type) {
            if(type == HistogramTypes.Point) {
                result[i].X = 0.5 * (minx + maxx);
                result[i].Y = y;
            }
            else if(type == HistogramTypes.Line) {
                result[2 * i].X = minx;
                result[2 * i].Y = y;
                result[2 * i + 1].X = maxx;
                result[2 * i + 1].Y = y;
            }
            else if(type == HistogramTypes.Bar) {
                result[4 * i].X = minx;
                result[4 * i].Y = 0;
                result[4 * i + 1].X = minx;
                result[4 * i + 1].Y = y;
                result[4 * i + 2].X = maxx;
                result[4 * i + 2].Y = y;
                result[4 * i + 3].X = maxx;
                result[4 * i + 3].Y = 0;
            }                
        }

        /// <summary>
        /// Vrátí hustotu vektoru
        /// </summary>
        /// <param name="intervals">Poèet intervalù</param>
        /// <param name="minX">Poèáteèní bod intervalu, od kterého se hustota poèítá</param>
        /// <param name="maxX">Koneèný bod intervalu, do kterého se hustota poèítá</param>
        public PointVector Density(int intervals, double minX, double maxX) {
			PointVector result = this.Histogram(intervals, minX, maxX);
			return result * new PointD(1, 1.0 / result.Integrate());
		}

		/// <summary>
		/// Vrátí odhad kumulovaného histogramu náhodného vektoru
		/// </summary>
		/// <param name="intervals">Poèet intervalù</param>
		/// <param name="min">Poèáteèní hodnota, od které se histogram poèítá</param>
		/// <param name="max">Maximální hodnota, do které se histogram poèítá</param>
		public PointVector CumulativeHistogram(int intervals, double min, double max) {
			if(this.Length == 0)
				throw new VectorException(errorMessageNoData);

			PointVector result = new PointVector(intervals);
			Vector sorted = this.Sort() as Vector;

			double step = (max - min) / intervals;

			int j = 0;
			for(int i = 0; i < intervals; i++) {
                result[i].X = min + step * ((double)i + 0.5);
				result[i].Y = i == 0 ? 0 : result[i - 1].Y;
				while((sorted[j] <= min + step * (i + 1)) && (j < sorted.Length - 1)) {
					result[i].Y++;
					j++;
				}
			}

			return result;
		}

        /// <summary>
        /// Vrátí kumulovaný histogram vektoru
        /// </summary>
        /// <param name="interval">Points of the interval</param>
        /// <returns></returns>
        public PointVector CumulativeHistogram(Vector interval) {
            int lengthR = interval.Length - 1;
            int length = this.Length;

            PointVector result = new PointVector(lengthR);

            for(int i = 0; i < lengthR; i++) {
                double minx = interval[i];
                double maxx = interval[i + 1];
                result[i].X = 0.5 * (minx + maxx);

                for(int j = 0; j < length; j++) {
                    double x = this[j];

                    if(x < maxx)
                        result[i].Y++;
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí pøesný kumulovaný histogram
        /// </summary>
        public PointVector CumulativeHistogram() {
            Vector v = this.Sort() as Vector;
            int length = v.Length;

            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++)
                result[i] = new PointD(v[i], i + 1);

            return result;
        }

        /// <summary>
        /// Kumulovaný histogram jako skokovou funkci
        /// </summary>
        public PointVector CumulativeHistogramStep() {
            Vector v = this.Sort() as Vector;
            int length = v.Length;

            ArrayList histogram = new ArrayList();
            double oldx = v[0];
            histogram.Add(new PointD(oldx, 0.0));
            for(int i = 1; i < length; i++)
                if(oldx != v[i]) {
                    histogram.Add(new PointD(oldx, i));
                    histogram.Add(new PointD(v[i], i));
                    oldx = v[i];
                }
            histogram.Add(new PointD(oldx, length));

            PointVector result = new PointVector(histogram.Count);
            int j = 0;
            foreach(PointD point in histogram)
                result[j++] = point;
            return result;
        }

        /// <summary>
        /// Vrátí odhad distribuèní funkce náhodného vektoru
        /// </summary>
        /// <param name="intervals">Poèet intervalù</param>
        /// <param name="minX">Poèáteèní bod intervalu, od kterého se distribuèní funkce poèítá</param>
        /// <param name="maxX">Koneèný bod intervalu, do kterého se distribuèní funkce poèítá</param>
        public PointVector DistributionFunction(int intervals, double minX, double maxX) {
			PointVector result = this.CumulativeHistogram(intervals, minX, maxX);
			return result * new PointD(1, 1.0 / result[intervals].Y);
		}

		/// <summary>
		/// Normalizuje daný vektor podle euklidovské normy
		/// </summary>
		public Vector EuklideanNormalization() {
			Vector result;
			result = this / this.EuklideanNorm();

			return result;
		}

		/// <summary>
		/// Normalizuje daný vektor podle absolutní normy
		/// </summary>
		public Vector AbsNormalization() {
			Vector result;
			result = this / this.AbsNorm();

			return result;
		}

		/// <summary>
		/// Normalizuje daný vektor tak, aby mìl nulový prùmìr jednotkový rozptyl
		/// </summary>
		public Vector StatisticalNormalization() {
			Vector result;
			result = (this - this.Mean()) / this.Variance();

			return result;
		}

        /// <summary>
        /// Absolutní hodnota vektoru
        /// </summary>
        public Vector Abs() {
            Vector result = new Vector(this.Length);

            for(int i = 0; i < result.Length; i++)
                result[i] = System.Math.Abs(this[i]);

            return result;
        }

        /// <summary>
        /// Spojí dva vektory
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
        /// Spojí vektory do jednoho
        /// </summary>
        /// <param name="vArray">Øada vektorù</param>
        public static Vector Join(params Vector[] vArray) {
			// Poèet prvkù výsledného vektoru
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
		/// Vypoèítá number of principal components
		/// </summary>
		public double NumPrincipalComponents() {
			double result = 0;

			int length = this.Length;
			for(int i = 0; i < length; i++) {
				double pow2 = this[i];
                pow2 *= pow2;
				result += pow2 * pow2;
			}
			result *= length;

			return 1.0 / result;
		}

        /// <summary>
        /// Entropie systému S = -Sum(ev[j]^2 ln(ev[j]^2))
        /// </summary>
        /// <remarks>M.S. Santhanam et al., arXiv:chao-dyn/9704002v1</remarks>
        public double Entropy() {
            double result = 0;

            int length = this.Length;
            for(int i = 0; i < length; i++) {
                double pow2 = this[i];
                pow2 *= pow2;
                if(pow2 != 0.0)
                    result -= pow2 * System.Math.Log(pow2);
            }

            return result;
        }

		/// <summary>
		/// Vypoèítá Symmetry parameter vektoru
		/// </summary>
		public double Symmetry() {
			double result = 0;
			
			int length = this.Length;
			for(int i = 0; i < length; i++)
				result += this[i] * System.Math.Abs(this[i]);

			return System.Math.Abs(result);
		}

        /// <summary>
        /// Vyhladí vektor tak, že vypoèítá prùmìr ze všech složek, které dané složce pøedcházejí
        /// </summary>
        public Vector FullSmooth() {
            int length = this.Length;
            Vector result = new Vector(length);

            double sum = 0.0;
            for(int i = 0; i < length; i++) {
                sum += this[i];
                result[i] = sum / (i + 1);
            }

            return result;
        }

        /// <summary>
        /// Provede jednoduché vyhlazení vektoru
        /// </summary>
        public Vector Smooth() {
            int length = this.Length;
            if(length <= 1)
                return this.Clone() as Vector;

            Vector result = new Vector(length);

            result[0] = (3.0 * this[0] + this[1]) / 4.0; ;
            for(int i = 1; i < length - 1; i++) 
                result[i] = (this[i - 1] + 2.0 * this[i] + this[i + 1]) / 4.0;
            result[length - 1] = (this[length - 2] + 3.0 * this[length - 1]) / 4.0;

            return result;
        }

        /// <summary>
        /// Pøevede vektor na øetìzec
        /// </summary>
        public override string ToString() {
			StringBuilder s = new StringBuilder();

			for(int i = 0; i < this.Length; i++) 
				s.Append(string.Format("{0,10:#####0.000}", this[i]));

			return s.ToString();
		}

        /// <summary>
        /// Creates a complex vector from given vector
        /// </summary>
        public ComplexVector ToComplexVector() {
            int length = this.Length;
            ComplexVector result = new ComplexVector(length);

            for(int i = 0; i < length; i++)
                result[i].Re = this[i];

            return result;
        }

        /// <summary>
        /// Detrended fluctuation analysis
        /// </summary>
        /// <param name="allPoints">All points for calculation</param>
        public PointVector DFA(bool allPoints) {
            int length = this.Length;
            int minlength = 4;
            int maxlength = length / 4;

            PointVector result = new PointVector(maxlength - minlength + 1);

            for(int i = minlength; i <= maxlength; i++) {
                PointVector v = new PointVector(i);
                for(int j = 0; j < i; j++)
                    v[j] = new PointD(j, 0.0);

                int numi = 
                    allPoints 
                    ? (length - 1) / i + 1
                    : length / i;

                double sqr = 0.0;

                for(int j = 0; j < numi; j++) {
                    int starti = 
                        allPoints
                        ? j * (length - i) / (numi - 1)
                        : j * i;

                    for(int k = 0; k < i; k++)
                        v[k].Y = this[starti + k];

                    Vector r = Regression.ComputeLinear(v);
                    sqr += r[5];
                }

                result[i - minlength] = new PointD(i, System.Math.Sqrt(sqr / (numi * i)));
            }

            return result;
        }

        private const string errorMessageNoData = "K provedení operace je nutné, aby délka vektoru nebyla nulová.";
		private const string errorMessageDifferentLength = "K provedení operace musí mít vektory stejnou délku.";
		private const string errorMessageMatrixVector = "Pøi násobení matice a vektoru musí mít oba objekty kompatibilní rozmìry.";
    }

	/// <summary>
	/// Výjimka ve tøídì Vector
	/// </summary>
	public class VectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public VectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public VectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve tøídì Vector došlo k chybì: ";
	}
}
