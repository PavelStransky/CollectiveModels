using System;
using System.IO;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// Implementace operací s komplexním vektorem
	/// </summary>
	public class ComplexVector: ICloneable, IExportable {
		// Prvky vektoru
		private Complex [] item;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public ComplexVector() { }
        
        /// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="length">Délka vektoru</param>
		public ComplexVector(int length) {
			this.item = new Complex[length];

			for(int i = 0; i < this.Length; i++)
				this.item[i] = new Complex();
		}
		
		/// <summary>
		/// Vytvoøí vektor s referencí na prvky
		/// </summary>
		/// <param name="item">Pole s prvky vektoru</param>
		public ComplexVector(Complex [] item) {
			this.item = item;
		}

		/// <summary>
		/// Poèet prvkù vektoru
		/// </summary>
		public int Length {get {return this.item.Length;}}

		/// <summary>
		/// Seète dva vektory stejných rozmìrù
		/// </summary>
		public static ComplexVector operator +(ComplexVector v1, ComplexVector v2) {
			if(v1.Length != v2.Length)
				throw new ComplexVectorException("Pøi sèítání vektorù musí mít vektory stejnou dimenzi.");

			ComplexVector result = new ComplexVector(v1.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = v1[i] + v2[i];

			return result;
		}

		/// <summary>
		/// Vynásobí vektor èíslem
		/// </summary>
		public static ComplexVector operator *(ComplexVector v, Complex koef) {
			ComplexVector result = new ComplexVector(v.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = koef * v[i];

			return result;
		}

		/// <summary>
		/// Skalární souèin
		/// </summary>
		public static ComplexVector operator *(ComplexVector v1, ComplexVector v2) {
			if(v1.Length != v2.Length)
				throw new ComplexVectorException(errorMessageDifferentLength);

			ComplexVector result = new ComplexVector(v1.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = v1[i] * !v2[i];

			return result;
		}

		/// <summary>
		/// Norma vektoru
		/// </summary>
		public double Norm() {
			double result = 0;
			for(int i = 0; i < this.Length; i++)
				result += this[i].SquaredNorm;

			return result;
		}

		/// <summary>
		/// Indexer
		/// </summary>
		public Complex this [int i] {
			get {
				return this.item[i];
			} 
			set {
				this.item[i] = value;
			}
		}

		/// <summary>
		/// Vektor vynuluje
		/// </summary>
		public void Clear() {
			for(int i = 0; i < this.Length; i++)
				this[i].Clear();
		}

		/// <summary>
		/// Implicitní pøetypování reálného vektoru na vektor komplexní
		/// </summary>
		public static implicit operator ComplexVector(Vector v) {
			ComplexVector result = new ComplexVector(v.Length);

			for(int i = 0; i < result.Length; i++) {
				result[i].Re = v[i];
				result[i].Im = 0.0;
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
                    b.Write(this[i].Im);
                    b.Write(this[i].Re);
                }
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
        public void Import(Core.Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                this.item = new Complex[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++) {
                    this[i].Re = b.ReadDouble();
                    this[i].Im = b.ReadDouble();
                }
            }
            else {
                // Textovì
                StreamReader t = import.T;
                this.item = new Complex[int.Parse(t.ReadLine())];
                for(int i = 0; i < this.Length; i++)
                    this[i] = new Complex(t.ReadLine());
            }
		}
		#endregion

		/// <summary>
		/// Øetìzec
		/// </summary>
		public override string ToString() {
			StringBuilder s = new StringBuilder();

			for(int i = 0; i < this.Length; i++) {
				s.Append(this[i].ToString());
				s.Append('\n');
			}

			return s.ToString();
		}

		/// <summary>
		/// Vytvoøí kopii vektoru
		/// </summary>
		public object Clone() {
			return new ComplexVector((Complex [])this.item.Clone());
		}

		private const string errorMessageDifferentLength = "K provedení operace musí mít vektory stejnou délku.";
	}

	/// <summary>
	/// Výjimka ve tøídì Vector
	/// </summary>
	public class ComplexVectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ComplexVectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ComplexVectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve tøídì ComplexVector došlo k chybì: ";
	}
}
