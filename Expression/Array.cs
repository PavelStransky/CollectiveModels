using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Øada - typová kontrola do objektu ArrayList
	/// </summary>
	public class TArray: ArrayList, IExportable {
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
		public TArray(bool checkSize) : base() {
			this.type = null;
			this.checkSize = checkSize;
		}

		/// <summary>
		/// Konstruktor (velikost objektù se kontroluje implicitnì)
		/// </summary>
		public TArray() : this(false) {}

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
        /// Vloží danou hodnotu do øady n-krát
        /// </summary>
        /// <param name="value">Objekt</param>
        /// <param name="n">Poèet opakování</param>
        /// <returns>Index posledního pøidaného objektu</returns>
        public int Add(object value, int n) {
            int result = -1;

            for(int i = 0; i < n; i++)
                result = this.Add(value);

            return result;
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
		/// Uloží obsah øady do souboru textovì
		/// </summary>
		/// <param name="export">Export</param>
        public void Export(Export export) {
            // Pokud ještì nebyl zadán typ, uložíme string.Empty
            string typeName = this.type != null ? this.type.FullName : string.Empty;

            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;
                b.Write(this.Count);
                b.Write(typeName);
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine(this.Count);
                t.WriteLine(typeName);
            }

            foreach(object o in this)
                export.Write(typeName, o);
        }

		/// <summary>
		/// Naète obsah øady ze souboru textovì
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            string typeName = string.Empty;
            int length = 0;
            this.Clear();

            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                length = b.ReadInt32();
                typeName = b.ReadString();
            }
            else {
                // Textovì
                StreamReader t = import.T;
                length = int.Parse(t.ReadLine());
                typeName = t.ReadLine();
            }

            if(length == 0)
                this.type = null;

            for(int i = 0; i < length; i++)
                this.Add(import.Read(typeName));
            
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
				else if(this.type == typeof(TArray)) {
					int itemLength = (item as TArray).Count;
					int thisLength = (this[0] as TArray).Count;
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
			TArray result = new TArray();
			foreach(object item in this)
				result.Add(item);
			return result;
		}

		/// <summary>
		/// Pøetypuje na øadu systému
		/// </summary>
		public static explicit operator System.Array(TArray array) {
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
		public static explicit operator TArray(System.Array array) {
			TArray result = new TArray();

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
