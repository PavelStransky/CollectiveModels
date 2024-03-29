using System;
using System.IO;
using System.Text;
using System.Numerics;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// Implementace operac� s komplexn�m vektorem
	/// </summary>
	public class ComplexVector: ICloneable, IExportable {
		// Prvky vektoru
		private Complex [] item;

        /// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="length">D�lka vektoru</param>
		public ComplexVector(int length) {
			this.item = new Complex[length];

			for(int i = 0; i < this.Length; i++)
				this.item[i] = new Complex();
		}
		
		/// <summary>
		/// Vytvo�� vektor s referenc� na prvky
		/// </summary>
		/// <param name="item">Pole s prvky vektoru</param>
		public ComplexVector(Complex [] item) {
			this.item = item;
		}

        /// <summary>
        /// Vektor re�ln�ch hodnot
        /// </summary>
        public Vector VectorRe {
            get {
                Vector result = new Vector(this.Length);

                for(int i = 0; i < result.Length; i++)
                    result[i] = this[i].Real;

                return result;
            }
            set {
                if(this.Length != value.Length)
                    throw new ComplexVectorException(errorMessageDifferentLength);

                for(int i = 0; i < this.Length; i++)
                    this[i] = new Complex(value[i], this[i].Imaginary);
            }
        }

        /// <summary>
        /// Vektor imagin�rn�ch hodnot
        /// </summary>
        public Vector VectorIm {
            get {
                Vector result = new Vector(this.Length);

                for(int i = 0; i < result.Length; i++)
                    result[i] = this[i].Imaginary;

                return result;
            }
            set {
                if(this.Length != value.Length)
                    throw new ComplexVectorException(errorMessageDifferentLength);

                for(int i = 0; i < this.Length; i++)
                    this[i] = new Complex(this[i].Real, value[i]);
            }
        }
        
        /// <summary>
		/// Po�et prvk� vektoru
		/// </summary>
		public int Length {get {return this.item.Length;}}

		/// <summary>
		/// Se�te dva vektory stejn�ch rozm�r�
		/// </summary>
		public static ComplexVector operator +(ComplexVector v1, ComplexVector v2) {
			if(v1.Length != v2.Length)
				throw new ComplexVectorException("P�i s��t�n� vektor� mus� m�t vektory stejnou dimenzi.");

			ComplexVector result = new ComplexVector(v1.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = v1[i] + v2[i];

			return result;
		}

		/// <summary>
		/// Vyn�sob� vektor ��slem
		/// </summary>
		public static ComplexVector operator *(ComplexVector v, Complex koef) {
			ComplexVector result = new ComplexVector(v.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = koef * v[i];

			return result;
		}

		/// <summary>
		/// Skal�rn� sou�in
		/// </summary>
		public static ComplexVector operator *(ComplexVector v1, ComplexVector v2) {
			if(v1.Length != v2.Length)
				throw new ComplexVectorException(errorMessageDifferentLength);

			ComplexVector result = new ComplexVector(v1.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = v1[i] * v2[i];

			return result;
		}

		/// <summary>
		/// Norma vektoru
		/// </summary>
		public double Norm() {
			double result = 0;
            for(int i = 0; i < this.Length; i++)
                result += System.Math.Pow(this[i].Magnitude, 2);

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
                this[i] = Complex.Zero;
		}

		/// <summary>
		/// Implicitn� p�etypov�n� re�ln�ho vektoru na vektor komplexn�
		/// </summary>
		public static implicit operator ComplexVector(Vector v) {
			ComplexVector result = new ComplexVector(v.Length);

			for(int i = 0; i < result.Length; i++) 
                result[i] = v[i];			
			
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
                    b.Write(this[i].Real);
                    b.Write(this[i].Imaginary);
                }
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
        public ComplexVector(Core.Import import) {
            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                this.item = new Complex[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++) {
                    this[i] = new Complex(b.ReadDouble(), b.ReadDouble());
                }
            }
		}
		#endregion

		/// <summary>
		/// �et�zec
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
		/// Vytvo�� kopii vektoru
		/// </summary>
		public object Clone() {
			return new ComplexVector((Complex [])this.item.Clone());
		}

		private const string errorMessageDifferentLength = "K proveden� operace mus� m�t vektory stejnou d�lku.";
	}

	/// <summary>
	/// V�jimka ve t��d� Vector
	/// </summary>
	public class ComplexVectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ComplexVectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ComplexVectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve t��d� ComplexVector do�lo k chyb�: ";
	}
}
