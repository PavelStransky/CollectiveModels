using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// Øada bodù (x, y)
	/// </summary>
	public class PointVector: ICloneable, IExportable, ISortable {
		// složky
		private PointD [] item;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="x">Vektor x - ových hodnot</param>
		/// <param name="y">Vektor y - ových hodnot</param>
		public PointVector(Vector x, Vector y) {
			int length = System.Math.Max(x.Length, y.Length);
			this.item = new PointD[length];

			for(int i = 0; i < this.item.Length; i++)
				this.item[i] = new PointD(x.Length > i ? x[i] : 0.0, y.Length > i ? y[i] : 0.0);
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="y">Vektor y - ových hodnot</param>
		/// <param name="dx">Diference v hodnotách x (implicitnì se zaèíná od 0)</param>
		public PointVector(double dx, Vector y) {
			this.item = new PointD[y.Length];

			for(int i = 0; i < this.item.Length; i++)
				this.item[i] = new PointD(i * dx, y[i]);
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="y">Vektor y - ových hodnot</param>
		public PointVector(Vector y) : this(1.0, y) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="item">Data jako øada bodù</param>
		public PointVector(PointD [] item) {
			this.item = item;
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="length">Délka vektoru</param>
		public PointVector(int length) {
			this.item = new PointD[length];
			for(int i = 0; i < this.Length; i++)
				this.item[i] = new PointD();
		}

		/// <summary>
		/// Indexer
		/// </summary>
		public PointD this[int i] {get {return this.item[i];} set {this.item[i] = value;}}

		/// <summary>
		/// Délka vektoru bodù
		/// </summary>
		public int Length {
            get {
                return this.item.Length;
            }

            set {
                PointD[] newItem = new PointD[value];

                int minLength = System.Math.Min(this.Length, value);
                for(int i = 0; i < minLength; i++)
                    newItem[i] = this.item[i];

                for(int i = minLength; i < value; i++)
                    newItem[i] = new PointD();

                this.item = newItem;
            }
        }
		
		/// <summary>
		/// Minimální x-ová hodnota
		/// </summary>
		public double MinX() {
			if(this.Length == 0)
				throw new PointVectorException(errorMessageNoData);

			double result = this[0].X;

			for(int i = 1; i < this.Length; i++)
				if(result > this[i].X)
					result = this[i].X;

			return result;
		}

		/// <summary>
		/// Maximální x-ová hodnota
		/// </summary>
		public double MaxX() {
			if(this.Length == 0)
				throw new PointVectorException(errorMessageNoData);

			double result = this[0].X;

			for(int i = 1; i < this.Length; i++)
				if(result < this[i].X)
					result = this[i].X;

			return result;
		}

		/// <summary>
		/// Minimální y-ová hodnota
		/// </summary>
		public double MinY() {
			if(this.Length == 0)
				throw new PointVectorException(errorMessageNoData);

			double result = this[0].Y;

			for(int i = 1; i < this.Length; i++)
				if(result > this[i].Y)
					result = this[i].Y;

			return result;
		}

		/// <summary>
		/// Maximální y-ová hodnota
		/// </summary>
		public double MaxY() {
			if(this.Length == 0)
				throw new PointVectorException(errorMessageNoData);

			double result = this[0].Y;

			for(int i = 1; i < this.Length; i++)
				if(result < this[i].Y)
					result = this[i].Y;

			return result;
		}

		/// <summary>
		/// Násobení vektoru bodem (x - ová složka se násobí hodnotou x, y - ová hodnotou y)
		/// </summary>
		/// <param name="pv">Vektor</param>
		/// <param name="point">Bod</param>
		public static PointVector operator * (PointVector pv, PointD point) {
			PointVector result = new PointVector(pv.Length);

			for(int i = 0; i < result.Length; i++) {
				result[i].X = pv[i].X * point.X;
				result[i].Y = pv[i].Y * point.Y;
			}

			return result;
		}

		/// <summary>
		/// Dìlení vektoru bodem (x - ová složka se násobí hodnotou x, y - ová hodnotou y)
		/// </summary>
		/// <param name="pv">Vektor</param>
		/// <param name="point">Bod</param>
		public static PointVector operator / (PointVector pv, PointD point) {
			PointVector result = new PointVector(pv.Length);

			for(int i = 0; i < result.Length; i++) {
				result[i].X = pv[i].X / point.X;
				result[i].Y = pv[i].Y / point.Y;
			}

			return result;
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
                for(int i = 0; i < this.Length; i++) {
                    b.Write(this[i].X);
                    b.Write(this[i].Y);
                }
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine(this.Length);
                for(int i = 0; i < this.Length; i++)
                    t.WriteLine("{0}\t{1}", this[i].X, this[i].Y);
            }
        }

		/// <summary>
		/// Naète obsah vektoru ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public PointVector(Core.Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                this.item = new PointD[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++)
                    this.item[i] = new PointD(b.ReadDouble(), b.ReadDouble());
            }
            else {
                // Textovì
                StreamReader t = import.T;
                this.item = new PointD[int.Parse(t.ReadLine())];
                for(int i = 0; i < this.Length; i++) {
                    string line = t.ReadLine();
                    string[] s = line.Split('\t');
                    this.item[i] = new PointD(double.Parse(s[0]), double.Parse(s[1]));
                }
            }
        }
		#endregion

		/// <summary>
		/// Provede vyhlazení
		/// </summary>
		/// <param name="interval">Interval k vyhlazení</param>
		public void Smooth(int interval) {
			PointD [] result = new PointD[this.Length];
			result[0] = new PointD(this[0].X, this[0].Y);

			// První hodnota
			for(int i = 1; i < this.Length; i++) {
				int i1 = i - (interval / 2);
				int i2 = i1 + interval;

				if(i1 < 0) i1 = 0;
				if(i2 >= this.Length) i2 = this.Length - 1;
				
				result[i] = new PointD(this[i].X, result[i-1].Y + (this[i2].Y - this[i1].Y) / interval);
			}

			this.item = result;
		}

		/// <summary>
		/// Provede integraci pod køivkou (lichobìžníkové pravidlo)
		/// </summary>
		public double Integrate() {
			double result = 0;
			PointVector sorted = this.SortX();

			for(int i = 1; i < this.Length; i++)
				result += (sorted[i].X - sorted[i - 1].X) * (sorted[i].Y + sorted[i - 1].Y) / 2.0;
			
			return result;
		}

		/// <summary>
		/// Vytvoøí kopii vektoru
		/// </summary>
		public object Clone() {
			return new PointVector((PointD [])this.item.Clone());
		}

        /// <summary>
        /// Prohodí souøadnice X, Y
        /// </summary>
        public PointVector SwapXY() {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++)
                result[i] = new PointD(this[i].Y, this[i].X);

            return result;
        }

        /// <summary>
        /// Vektor x-ových hodnot
        /// </summary>
        public Vector VectorX {
			get {
				Vector result = new Vector(this.Length);

				for(int i = 0; i < result.Length; i++)
					result[i] = this[i].X;

				return result;
			}
			set {
				if(this.Length != value.Length)
					throw new PointVectorException(errorMessageDifferentLength);

				for(int i = 0; i < this.Length; i++)
					this[i].X = value[i];
			}
		}

		/// <summary>
		/// Vektor y-ových hodnot
		/// </summary>
		public Vector VectorY {
			get {
				Vector result = new Vector(this.Length);

				for(int i = 0; i < result.Length; i++)
					result[i] = this[i].Y;

				return result;
			}
			set {
				if(this.Length != value.Length)
					throw new PointVectorException(errorMessageDifferentLength);

				for(int i = 0; i < this.Length; i++)
					this[i].Y = value[i];
			}
        }

        #region Tøídìní
        /// <summary>
        /// Keys for sorting
        /// </summary>
        public Array GetKeys() {
            return this.VectorY.GetKeys();
        }

        /// <summary>
		/// Tøídìní podle X
		/// </summary>
		public PointVector SortX() {
            return this.Sort(this.VectorX) as PointVector;
		}

		/// <summary>
		/// Tøídìní podle Y
		/// </summary>
		public PointVector SortY() {
            return this.Sort(this.VectorY) as PointVector;
        }

        /// <summary>
        /// Tøídìní vzestupnì
        /// </summary>
        /// <returns></returns>
        public object Sort() {
            return this.SortX();
        }

        /// <summary>
        /// Tøídìní sestupnì
        /// </summary>
        /// <returns></returns>
        public object SortDesc() {
            PointVector result = this.Sort() as PointVector;
            System.Array.Reverse(result.item);
            return result;
        }

        /// <summary>
        /// Tøídìní vzestupnì s klíèi
        /// </summary>
        /// <param name="keys">Klíèe</param>
        /// <returns></returns>
        public object Sort(ISortable keys) {
            PointVector result = this.Clone() as PointVector;
            Array.Sort(keys.GetKeys(), result.item);
            return result;
        }

        /// Použije klíèe k setøídìní vektoru sestupnì
        /// </summary>
        /// <param name="keys">Klíèe k setøídìní</param>
        public object SortDesc(ISortable keys) {
            PointVector result = this.Sort(keys) as PointVector;
            System.Array.Reverse(result.item);
            return result;
        }
        #endregion

        /// <summary>
		/// Pøehodí x-ovou a y-ovou složku vektoru
		/// </summary>
		public PointVector Transposition() {
			PointVector result = new PointVector(this.Length);

			for(int i = 0; i < this.Length; i++)
				result[i] = new PointD(this[i].Y, this[i].X);
			
			return result;
		}

		/// <summary>
		/// Normalizuje daný vektor vzhledem k hodnotám Y
		/// </summary>
		public PointVector Normalization() {
			PointVector result = new PointVector(this.Length);

			double sum = 0;
			for(int i = 0; i < this.Length; i++)
				sum += this[i].Y;

			for(int i = 0; i < this.Length; i++) 
				result[i] = new PointD(this[i].X, this[i].Y / sum);

			return result;
		}

		/// <summary>
		/// Perform the tranformation of all components of the vector
		/// </summary>
		/// <param name="fx">Transformation function x</param>
		/// <param name="fy">Transformation function y</param>
		public PointVector Transform(RealFunction fx, RealFunction fy) {
            int length = this.Length;
			PointVector result = new PointVector(length);

			for(int i = 0; i < length; i++) {
				if(fx != null)
					result[i].X = fx(this[i].X);
				else
					result[i].X = this[i].X;

				if(fy != null)
					result[i].Y = fy(this[i].Y);
				else
					result[i].Y = this[i].Y;
			}

			return result;
		}

        /// <summary>
        /// Perform the tranformation of all components of the vector
        /// </summary>
        /// <param name="f">Transformation function y</param>
        public PointVector Transform(RealFunction f) {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++) {
                double x = this[i].X;
                result[i].X = this[i].X;
                result[i].Y = f(this[i].Y);
            }

            return result;
        }

        /// <summary>
        /// Perform the tranformation of all components of the vector
        /// </summary>
        /// <param name="f">Transformation function y</param>
        /// <param name="p">Additional parameters for the transformation function</param>
        public PointVector Transform(RealFunctionWithParams f, params object[] p) {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++) {
                double x = this[i].X;
                result[i].X = this[i].X;
                result[i].Y = f(this[i].Y, p);
            }

            return result;
        }
        
        /// <summary>
        /// Spojí vektory do jednoho
        /// </summary>
        /// <param name="vArray">Øada vektorù</param>
        public static PointVector Join(PointVector[] vArray) {
            // Poèet prvkù výsledného vektoru
            int numItems = 0;
            for(int i = 0; i < vArray.Length; i++)
                numItems += vArray[i].Length;

            PointVector result = new PointVector(numItems);

            int item = 0;
            for(int i = 0; i < vArray.Length; i++)
                for(int j = 0; j < vArray[i].Length; j++)
                    result[item++] = vArray[i][j];

            return result;
        }

        /// <summary>
        /// Najde všechny body, ve kterých se dané køivky protínají
        /// </summary>
        /// <param name="v">Druhá køivka</param>
        public PointVector Intersection(PointVector v) {
            int length1 = this.Length;
            int length2 = v.Length;

            ArrayList r = new ArrayList();

            for(int i = 1; i < length1; i++) {
                double x1min = this[i - 1].X;
                double y1min = this[i - 1].Y;
                double x1max = this[i].X;
                double y1max = this[i].Y;

                for(int j = 1; j < length2; j++) {
                    double x2min = v[j - 1].X;
                    double y2min = v[j - 1].Y;
                    double x2max = v[j].X;
                    double y2max = v[j].Y;

                    double d = (x1max - x1min) * (y2max - y2min) - (y1max - y1min) * (x2max - x2min);
                    double x = ((y1min * x1max - y1max * x1min) * (x2max - x2min) + (y2max * x2min - y2min * x2max) * (x1max - x1min));
                    x /= d;
                    double y = ((y1min * x1max - y1max * x1min) * (y2max - y2min) + (y2max * x2min - y2min * x2max) * (y1max - y1min));
                    y /= d;

/*                  ((bc-ad)(h-g)+(d-c)(fg-eh))/((d-c)(f-e)-(b-a)(h-g))
                    ((ad-bc)(f-e)+(b-a)(fg-eh))/((d-c)(f-e)-(b-a)(h-g))

                    x -> (a d (g - h) + b c (-g + h) + (c - d) (f g - e h))/(
                     d (e - f) + c (-e + f) + (a - b) (g - h)), y -> (
                     a (d (e - f) + f g - e h) + b (c (-e + f) - f g + e h))/(
                     d (e - f) + c (-e + f) + (a - b) (g - h)) */
                    
                    // Existuje prùnik
                    if(((x >= x1min && x <= x1max) || (x >= x1max && x <= x1min) || (x1min == x1max)) && ((y >= y1min && y <= y1max) || (y >= y1max && y <= y1min) || (y1min == y1max))
                        && ((x >= x2min && x <= x2max) || (x >= x2max && x <= x2min) || (x2min == x2max)) && ((y >= y2min && y <= y2max) || (y >= y2max && y <= y2min) || (y2min == y2max)))
                        r.Add(new PointD(x, y));
                }
            }

            int count = r.Count;
            PointVector result = new PointVector(count);
            for(int i = 0; i < count; i++)
                result[i] = (PointD)r[i];

            return result;
        }

        /// <summary>
        /// Vrací true, pokud je daný bod uvnitø køivky zadané vektorem bodù
        /// </summary>
        /// <param name="point">Bod, který zkoumáme</param>
        /// <remarks>Algoritmus - Point in polygon</remarks>
        public bool IsInside(PointD point) {
            int length = this.Length;
            bool result = false;

            double x = point.X;
            double y = point.Y;

            int j = length - 1;

            for(int i = 0; i < length; i++) {                
                if((this[i].Y < y && this[j].Y >= y) || (this[j].Y < y && this[i].Y >= y)) {
                    if(this[i].X + (y - this[i].Y) / (this[j].Y - this[i].Y) * (this[j].X - this[i].X) < x)
                        result = !result;                    
                }
                j = i;
            }

            return result;
        }

        /// <summary>
        /// Pointvector pøetvoøí do tvaru schodu
        /// </summary>
        public PointVector StairsX() {
            int length = this.Length;
            if(length <= 1)
                return (PointVector)this.Clone();

            double x = this[0].X - (this[1].X - this[0].X) / 2;

            PointVector result = new PointVector(2 * length);
            for(int i = 0; i < length; i++) {
                result[2 * i] = new PointD(x, this[i].Y);
                if(i < length - 1)
                    x = this[i].X + (this[i + 1].X - this[i].X) / 2;
                else
                    x = this[i].X + (this[i].X - this[i - 1].X) / 2;
                result[2 * i + 1] = new PointD(x, this[i].Y);
            }
            return result;
        }

        /// <summary>
        /// Vektor pøevede na textový øetìzec
        /// </summary>
        public override string ToString() {
			StringBuilder result = new StringBuilder();

			for(int i = 0; i < this.Length; i++) {
				result.Append(this[i].ToString());
				result.Append("\r\n");
			}
			
			return result.ToString();
		}


		private const string errorMessageNoData = "K provedení operace je nutné, aby délka vektoru nebyla nulová.";
		private const string errorMessageDifferentLength = "K provedení operace musí mít vektory stejnou délku.";
    }

	/// <summary>
	/// Výjimka ve tøídì Vector
	/// </summary>
	public class PointVectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public PointVectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public PointVectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve tøídì PointVector došlo k chybì: ";
	}
}
