using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Øada - typová kontrola do objektu ArrayList
	/// </summary>
	public class Array: ArrayList, IExportable {
		// Typ objektù v øadì (nastaví se poprvé, pak už zùstává nemìnný)
		private Type type;
		// Provádìní kontroly na stejnost pøidávaných objektù
		private bool checkSize;

		/// <summary>
		/// Typ objektù v øadì
		/// </summary>
		public Type ItemType {get {return this.type;}}

		/// <summary>
		/// Název typu objektù v øadì
		/// </summary>
		public string ItemTypeName {get {return this.type.FullName;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="checkSize">Bude se kontrolovat velikost objektu</param>
		public Array(bool checkSize) : base() {
			this.type = null;
			this.checkSize = checkSize;
		}

		/// <summary>
		/// Konstruktor (velikost objektù se kontroluje implicitnì)
		/// </summary>
		public Array() : this(true) {}

		/// <summary>
		/// Pøidá objekt na konec øady
		/// </summary>
		/// <param name="item">Objekt</param>
		/// <returns>Index novì pøidaného objektu</returns>
		public override int Add(object value) {
			// Kontrola na typ
			this.CheckType(value);
			// Kontrola na velikost
			this.CheckSize(value);

			return base.Add(value);
		}

		/// <summary>
		/// Pøidá objekt na zadaný index
		/// </summary>
		/// <param name="item">Objekt</param>
		/// <param name="index">Index pøidávaného objektu</param>
		public override void Insert(int index, object item) {
			// Kontrola na typ
			this.CheckType(item);
			// Kontrola na velikost
			this.CheckSize(item);

			base.Insert(index, item);
		}
		
		/// <summary>
		/// Indexer
		/// </summary>
		/// 
		public override object this[int i] {
			get {
				return base[i];
			}
			set {
				this.CheckType(value);
				this.CheckSize(value);
				
				base[i] = value;
			}
		}

		#region Implementace IExportable
		/// <summary>
		/// Vytvoøí øadu z dat ze souboru
		/// </summary>
		/// <param name="fileName">Jméno souboru</param>
		/// <param name="binary">Soubor v binární podobì</param>
		public Array(string fileName, bool binary) {
			this.Import(fileName, binary);
		}

		/// <summary>
		/// Vytvoøí øadu ze StreamReaderu
		/// </summary>
		/// <param name="t">StreamReader</param>
		public Array(StreamReader t) {
			this.Import(t);
		}
		
		/// <summary>
		/// Vytvoøí øadu z BinaryReaderu
		/// </summary>
		/// <param name="b">BinaryReader</param>
		public Array(BinaryReader b) {
			this.Import(b);
		}

		/// <summary>
		/// Uloží obsah øady do souboru
		/// </summary>
		/// <param name="fName">Jméno souboru</param>
		/// <param name="binary">Ukládat v binární podobì</param>
		public void Export(string fName, bool binary) {
			FileStream f = new FileStream(fName, FileMode.Create);

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
		/// Uloží obsah øady do souboru textovì
		/// </summary>
		/// <param name="t">StreamWriter</param>
		public void Export(StreamWriter t) {
			t.WriteLine(this.GetType().FullName);
			t.WriteLine(this.Count);
			t.WriteLine(this.type.FullName);
			for(int i = 0; i < this.Count; i++) {
				if(this[i] is IExportable)
					(this[i] as IExportable).Export(t);
				else if(this[i] is double || this[i] is int || this[i] is string) {
					t.WriteLine(this[i]);
				}
			}
		}

		/// <summary>
		/// Uloží obsah øady do souboru binárnì
		/// </summary>
		/// <param name="b">BinaryWriter</param>
		public void Export(BinaryWriter b) {
			b.Write(this.GetType().FullName);
			b.Write(this.Count);
			b.Write(this.type.FullName);
			for(int i = 0; i < this.Count; i++) {
				if(this[i] is IExportable)
					(this[i] as IExportable).Export(b);
				else if(this[i] is double) 
					b.Write((double)this[i]);
				else if(this[i] is int)
					b.Write((int)this[i]);
				else if(this[i] is string) 
					b.Write((string)this[i]);
			}
		}

		/// <summary>
		/// Naète obsah øady ze souboru
		/// </summary>
		/// <param name="fName">Jméno souboru</param>
		/// <param name="binary">Soubor v binární podobì</param>
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
		/// Naète obsah øady ze souboru textovì
		/// </summary>
		/// <param name="t">StreamReader</param>
		public void Import(StreamReader t) {
			ImportExportException.CheckImportType(t.ReadLine(), this.GetType());			

			int length = int.Parse(t.ReadLine());
			string typeName = t.ReadLine();
			if(length == 0)
				this.type = null;

			for(int i = 0; i < length; i++) {
                if(typeName == typeof(Vector).FullName)
                    this.Add(new Vector(t));
                else if(typeName == typeof(Matrix).FullName)
                    this.Add(new Matrix(t));
                else if(typeName == typeof(Array).FullName)
                    this.Add(new Array(t));
                else if(typeName == typeof(PointD).FullName)
                    this.Add(new PointD(t));
                else if(typeName == typeof(PointVector).FullName)
                    this.Add(new PointVector(t));
                else if(typeName == typeof(double).FullName)
                    this.Add(double.Parse(t.ReadLine()));
                else if(typeName == typeof(int).FullName)
                    this.Add(int.Parse(t.ReadLine()));
                else if(typeName == typeof(string).FullName)
                    this.Add(t.ReadLine());
			}
		}


		/// <summary>
		/// Naète obsah øady ze souboru binárnì
		/// </summary>
		/// <param name="b">BinaryReader</param>
		public void Import(BinaryReader b) {
			ImportExportException.CheckImportType(b.ReadString(), this.GetType());

			int length = b.ReadInt32();
			string typeName = b.ReadString();
			if(length == 0)
				this.type = null;

			for(int i = 0; i < length; i++) {
                if(typeName == typeof(Vector).FullName)
                    this.Add(new Vector(b));
                else if(typeName == typeof(Matrix).FullName)
                    this.Add(new Matrix(b));
                else if(typeName == typeof(Array).FullName)
                    this.Add(new Array(b));
                else if(typeName == typeof(PointD).FullName)
                    this.Add(new PointD(b));
                else if(typeName == typeof(PointVector).FullName)
                    this.Add(new PointVector(b));
                else if(typeName == typeof(double).FullName)
                    this.Add(b.ReadDouble());
                else if(typeName == typeof(int).FullName)
                    this.Add(b.ReadInt32());
                else if(typeName == typeof(string).FullName)
                    this.Add(b.ReadString());
			}
		}
		#endregion

		/// <summary>
		/// Provede typovou kontrolu pøidávaného objektu
		/// </summary>
		/// <param name="item">Pøidávaný objekt</param>
		private void CheckType(object item) {
			if(this.type == null) {
				this.type = item.GetType();
			}
			else {
				if(this.type != item.GetType())
					throw new ContextException(errorMessageBadType, string.Format(errorMessageBadTypeDetail, item.GetType().FullName, this.type.FullName));
			}
		}

		/// <summary>
		/// Provede kontrolu na velikost pøidávaného objektu
		/// </summary>
		/// <param name="item">Pøidávaný objekt</param>
		/// <param name="name">Jméno</param>
		private void CheckSize(object item) {
			if(this.checkSize && this.Count > 0) {
				string errorMessage = null;

				// Zkontrolujeme objekty, které zkontrolovat umíme
				if(this.type == typeof(Vector)) {
					int itemLength = (item as Vector).Length;
					int thisLength = (this[0] as Vector).Length;
					if(itemLength != thisLength)
						errorMessage = string.Format(errorMessageBadSizeDetail, "{0}", "{1}", itemLength, thisLength);
				}
				else if(this.type == typeof(Matrix)) {
					int itemLengthX = (item as Matrix).LengthX;
					int itemLengthY = (item as Matrix).LengthY;
					int thisLengthX = (this[0] as Matrix).LengthX;
					int thisLengthY = (this[0] as Matrix).LengthY;
					if(itemLengthX != thisLengthX && itemLengthY != thisLengthY)
						errorMessage = string.Format(errorMessageBadSizeDetail, "{0}", "{1}", string.Format("{0} x {1}", itemLengthX, itemLengthY), string.Format("{0} x {1}", thisLengthX, thisLengthY));
				}
				else if(this.type == typeof(Array)) {
					int itemLength = (item as Array).Count;
					int thisLength = (this[0] as Array).Count;
					if(itemLength != thisLength)
						errorMessage = string.Format(errorMessageBadSizeDetail, "{0}", "{1}", itemLength, thisLength);
				}

				if(errorMessage != null)
					throw new ContextException(errorMessageBadSize, string.Format(errorMessage, this.type.FullName));
			}
		}

		/// <summary>
		/// Hodnota jako øetìzec
		/// </summary>
		public override string ToString() {
			StringBuilder s = new StringBuilder();
			for(int i = 0; i < this.Count; i++) {
				s.Append(this[i].ToString());
				s.Append('\n');
			}
			return s.ToString();
		}

		/// <summary>
		/// Klonování øady
		/// </summary>
		public override object Clone() {
			Array result = new Array();
			foreach(object item in this)
				result.Add(item);
			return result;
		}

		/// <summary>
		/// Pøetypuje na øadu systému
		/// </summary>
		public static explicit operator System.Array(Array array) {
			if(array.type == null)
				throw new ContextException(errorMessageArrayNotInitialized);

			System.Array result = System.Array.CreateInstance(array.type, array.Count);
			for(int i = 0; i < result.Length; i++)
				result.SetValue(array[i], i);

			return result;
		}

		/// <summary>
		/// Pøetypuje øadu systému na naši øadu
		/// </summary>
		public static explicit operator Array(System.Array array) {
			Array result = new Array();

			for(int i = 0; i < array.Length; i++)
				result.Add(array.GetValue(i));

			return result;
		}

		private const string errorMessageArrayNotInitialized = "Øada nebyla inicializována, neznáme typ prvkù. Požadovanou operaci nelze provést.";

		private const string errorMessageBadType = "Nový objekt pøidávaný do øady má s ostatními objekty nekonzistentní typ.";
		private const string errorMessageBadTypeDetail = "\nTyp: {0}\nTyp øady: {1}";

		private const string errorMessageBadSize = "Nový objekt pøidávaný do øady nemá stejnou velikost jako ostatní objekty.";
		private const string errorMessageBadSizeDetail = "\nTyp: {0}\nVelikost: {1}\nVelikost v øadì: {2}";
	}
}
