using System;
using System.IO;
using System.Text;

namespace PavelStransky.Math {
	/// <summary>
	/// �ada bod� (x, y)
	/// </summary>
	public class PointVector: ICloneable, IExportable, ISortable {
		// slo�ky
		private PointD [] item;

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public PointVector() { }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="x">Vektor x - ov�ch hodnot</param>
		/// <param name="y">Vektor y - ov�ch hodnot</param>
		public PointVector(Vector x, Vector y) {
			int length = System.Math.Max(x.Length, y.Length);
			this.item = new PointD[length];

			for(int i = 0; i < this.item.Length; i++)
				this.item[i] = new PointD(x.Length > i ? x[i] : 0.0, y.Length > i ? y[i] : 0.0);
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="y">Vektor y - ov�ch hodnot</param>
		/// <param name="dx">Diference v hodnot�ch x (implicitn� se za��n� od 0)</param>
		public PointVector(double dx, Vector y) {
			this.item = new PointD[y.Length];

			for(int i = 0; i < this.item.Length; i++)
				this.item[i] = new PointD(i * dx, y[i]);
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="y">Vektor y - ov�ch hodnot</param>
		public PointVector(Vector y) : this(1.0, y) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="item">Data jako �ada bod�</param>
		public PointVector(PointD [] item) {
			this.item = item;
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="length">D�lka vektoru</param>
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
		/// D�lka vektoru bod�
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
		/// Minim�ln� x-ov� hodnota
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
		/// Maxim�ln� x-ov� hodnota
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
		/// Minim�ln� y-ov� hodnota
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
		/// Maxim�ln� y-ov� hodnota
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
		/// N�soben� vektoru bodem (x - ov� slo�ka se n�sob� hodnotou x, y - ov� hodnotou y)
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
		/// D�len� vektoru bodem (x - ov� slo�ka se n�sob� hodnotou x, y - ov� hodnotou y)
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
		/// Ulo�� obsah vektoru do souboru
		/// </summary>
		/// <param name="export">Export</param>
		public void Export(Export export) {
            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(this.Length);
                for(int i = 0; i < this.Length; i++) {
                    b.Write(this[i].X);
                    b.Write(this[i].Y);
                }
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine(this.Length);
                for(int i = 0; i < this.Length; i++)
                    t.WriteLine("{0}\t{1}", this[i].X, this[i].Y);
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
                this.item = new PointD[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++)
                    this.item[i] = new PointD(b.ReadDouble(), b.ReadDouble());
            }
            else {
                // Textov�
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
		/// Provede vyhlazen�
		/// </summary>
		/// <param name="interval">Interval k vyhlazen�</param>
		public void Smooth(int interval) {
			PointD [] result = new PointD[this.Length];
			result[0] = new PointD(this[0].X, this[0].Y);

			// Prvn� hodnota
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
		/// Provede integraci pod k�ivkou (lichob�n�kov� pravidlo)
		/// </summary>
		public double Integrate() {
			double result = 0;
			PointVector sorted = this.SortX();

			for(int i = 1; i < this.Length; i++)
				result += (sorted[i].X - sorted[i - 1].X) * (sorted[i].Y + sorted[i - 1].Y) / 2.0;
			
			return result;
		}

		/// <summary>
		/// Vytvo�� kopii vektoru
		/// </summary>
		public object Clone() {
			return new PointVector((PointD [])this.item.Clone());
		}

		/// <summary>
		/// Vektor x-ov�ch hodnot
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
		/// Vektor y-ov�ch hodnot
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

        #region T��d�n�
        /// <summary>
		/// T��d�n� podle X
		/// </summary>
		public PointVector SortX() {
			return new PointVector(this.VectorX.Sort(this.item) as PointD []);
		}

		/// <summary>
		/// T��d�n� podle Y
		/// </summary>
		public PointVector SortY() {
			return new PointVector(this.VectorY.Sort(this.item) as PointD []);
        }

        /// <summary>
        /// T��d�n� vzestupn�
        /// </summary>
        /// <returns></returns>
        public object Sort() {
            return this.SortX();
        }

        /// <summary>
        /// T��d�n� sestupn�
        /// </summary>
        /// <returns></returns>
        public object SortDesc() {
            PointVector result = this.Sort() as PointVector;
            System.Array.Reverse(result.item);
            return result;
        }

        /// <summary>
        /// T��d�n� vzestupn� s kl��i
        /// </summary>
        /// <param name="keys">Kl��e</param>
        /// <returns></returns>
        public object Sort(Vector keys) {
            return new PointVector(keys.Sort(this.item) as PointD[]);
        }

        /// Pou�ije kl��e k set��d�n� vektoru sestupn�
        /// </summary>
        /// <param name="keys">Kl��e k set��d�n�</param>
        public object SortDesc(Vector keys) {
            PointVector result = this.Sort(keys) as PointVector;
            System.Array.Reverse(result.item);
            return result;
        }
        #endregion

        /// <summary>
		/// P�ehod� x-ovou a y-ovou slo�ku vektoru
		/// </summary>
		public PointVector Transposition() {
			PointVector result = new PointVector(this.Length);

			for(int i = 0; i < this.Length; i++)
				result[i] = new PointD(this[i].Y, this[i].X);
			
			return result;
		}

		/// <summary>
		/// Normalizuje dan� vektor vzhledem k hodnot�m Y
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
		/// Provede transformaci v�ech slo�ek vektoru podle dan�ch funkc�
		/// </summary>
		/// <param name="fx">Transforma�n� funkce x</param>
		/// <param name="fy">Transforma�n� funkce y</param>
		public PointVector Transform(RealFunction fx, RealFunction fy) {
			PointVector result = new PointVector(this.Length);

			for(int i = 0; i < result.Length; i++) {
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
		/// Vektor p�evede na textov� �et�zec
		/// </summary>
		public override string ToString() {
			StringBuilder result = new StringBuilder();

			for(int i = 0; i < this.Length; i++) {
				result.Append(this[i].ToString());
				result.Append("\r\n");
			}
			
			return result.ToString();
		}


		private const string errorMessageNoData = "K proveden� operace je nutn�, aby d�lka vektoru nebyla nulov�.";
		private const string errorMessageDifferentLength = "K proveden� operace mus� m�t vektory stejnou d�lku.";
	}

	/// <summary>
	/// V�jimka ve t��d� Vector
	/// </summary>
	public class PointVectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public PointVectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public PointVectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve t��d� PointVector do�lo k chyb�: ";
	}
}
