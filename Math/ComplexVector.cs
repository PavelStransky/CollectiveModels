using System;
using System.IO;
using System.Text;

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
		/// Implicitn� p�etypov�n� re�ln�ho vektoru na vektor komplexn�
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
		/// Ulo�� obsah vektoru do souboru
		/// </summary>
		/// <param name="fileName">Jm�no souboru</param>
		/// <param name="binary">Ukl�dat v bin�rn� podob�</param>
		public void Export(string fileName, bool binary) {
			FileStream f = new FileStream(fileName, FileMode.Create);

			if(binary) {
				BinaryWriter b = new BinaryWriter(f);
				this.Export(b);
				b.Close();
			}
			else {
				StreamWriter t = new StreamWriter(f);
				this.Export(t);
				t.Close();
			}

			f.Close();
		}

		/// <summary>
		/// Ulo�� obsah vektoru do souboru textov�
		/// </summary>
		/// <param name="t">StreamWriter</param>
		public void Export(StreamWriter t) {
			t.WriteLine(this.GetType().FullName);
			t.WriteLine(this.Length);

			for(int i = 0; i < this.Length; i++)
				t.WriteLine(this[i]);
		}

		/// <summary>
		/// Ulo�� obsah vektoru do souboru bin�rn�
		/// </summary>
		/// <param name="b">BinaryWriter</param>
		public void Export(BinaryWriter b) {
			b.Write(this.GetType().FullName);
			b.Write(this.Length);

			for(int i = 0; i < this.Length; i++) {
				b.Write(this[i].Im);
				b.Write(this[i].Re);
			}
		}

		/// <summary>
		/// Na�te obsah vektoru ze souboru
		/// </summary>
		/// <param name="fName">Jm�no souboru</param>
		/// <param name="binary">Soubor v bin�rn� podob�</param>
		public void Import(string fName, bool binary) {
			FileStream f = new FileStream(fName, FileMode.Open);

			if(binary) {
				BinaryReader b = new BinaryReader(f);
				this.Import(b);
				b.Close();
			}
			else {
				StreamReader t = new StreamReader(f);
				this.Import(t);
				t.Close();
			}

			f.Close();
		}
		
		/// <summary>
		/// Na�te obsah vektoru ze souboru
		/// </summary>
		/// <param name="t">StreamReader</param>
		public void Import(StreamReader t) {
			ImportExportException.CheckImportType(t.ReadLine(), this.GetType());

			this.item = new Complex[int.Parse(t.ReadLine())];
			for(int i = 0; i < this.Length; i++)
				this[i] = new Complex(t.ReadLine());
		}

		/// <summary>
		/// Na�te obsah vektoru ze souboru bin�rn�
		/// </summary>
		/// <param name="b">BinaryReader</param>
		public void Import(BinaryReader b) {
			ImportExportException.CheckImportType(b.ReadString(), this.GetType());

			this.item = new Complex[b.ReadInt32()];
			for(int i = 0; i < this.Length; i++) {
				this[i].Re = b.ReadDouble();
				this[i].Im = b.ReadDouble();
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
