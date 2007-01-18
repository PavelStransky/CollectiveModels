using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// �ada - typov� kontrola do objektu ArrayList
	/// </summary>
	public class TArray: ArrayList, IExportable {
		// Typ objekt� v �ad� (nastav� se poprv�, pak u� z�st�v� nem�nn�)
		private Type type;
		// Prov�d�n� kontroly na stejnost p�id�van�ch objekt�
		private bool checkSize;

		/// <summary>
		/// Typ objekt� v �ad�
		/// </summary>
		public Type ItemType {get {return this.type;}}

		/// <summary>
		/// N�zev typu objekt� v �ad�
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
		/// Konstruktor (velikost objekt� se kontroluje implicitn�)
		/// </summary>
		public TArray() : this(false) {}

		/// <summary>
		/// P�id� objekt na konec �ady
		/// </summary>
		/// <param name="item">Objekt</param>
		/// <returns>Index nov� p�idan�ho objektu</returns>
		public override int Add(object value) {
			// Kontrola na typ
			this.CheckType(value);
			// Kontrola na velikost
			this.CheckSize(value);

			return base.Add(value);
		}

        /// <summary>
        /// Vlo�� danou hodnotu do �ady n-kr�t
        /// </summary>
        /// <param name="value">Objekt</param>
        /// <param name="n">Po�et opakov�n�</param>
        /// <returns>Index posledn�ho p�idan�ho objektu</returns>
        public int Add(object value, int n) {
            int result = -1;

            for(int i = 0; i < n; i++)
                result = this.Add(value);

            return result;
        }

        /// <summary>
        /// P�id� objekt na zadan� index
        /// </summary>
        /// <param name="item">Objekt</param>
        /// <param name="index">Index p�id�van�ho objektu</param>
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
		/// Ulo�� obsah �ady do souboru textov�
		/// </summary>
		/// <param name="export">Export</param>
        public void Export(Export export) {
            // Pokud je�t� nebyl zad�n typ, ulo��me string.Empty
            string typeName = this.type != null ? this.type.FullName : string.Empty;

            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(this.Count);
                b.Write(typeName);
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine(this.Count);
                t.WriteLine(typeName);
            }

            foreach(object o in this)
                export.Write(typeName, o);
        }

		/// <summary>
		/// Na�te obsah �ady ze souboru textov�
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            string typeName = string.Empty;
            int length = 0;
            this.Clear();

            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                length = b.ReadInt32();
                typeName = b.ReadString();
            }
            else {
                // Textov�
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
		/// Provede typovou kontrolu p�id�van�ho objektu
		/// </summary>
		/// <param name="item">P�id�van� objekt</param>
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
		/// Provede kontrolu na velikost p�id�van�ho objektu
		/// </summary>
		/// <param name="item">P�id�van� objekt</param>
		/// <param name="name">Jm�no</param>
		private void CheckSize(object item) {
			if(this.checkSize && this.Count > 0) {
				string errorMessage = null;

				// Zkontrolujeme objekty, kter� zkontrolovat um�me
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
		/// Hodnota jako �et�zec
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
		/// Klonov�n� �ady
		/// </summary>
		public override object Clone() {
			TArray result = new TArray();
			foreach(object item in this)
				result.Add(item);
			return result;
		}

		/// <summary>
		/// P�etypuje na �adu syst�mu
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
		/// P�etypuje �adu syst�mu na na�i �adu
		/// </summary>
		public static explicit operator TArray(System.Array array) {
			TArray result = new TArray();

			for(int i = 0; i < array.Length; i++)
				result.Add(array.GetValue(i));

			return result;
		}

		private const string errorMessageArrayNotInitialized = "�ada nebyla inicializov�na, nezn�me typ prvk�. Po�adovanou operaci nelze prov�st.";

		private const string errorMessageBadType = "Nov� objekt p�id�van� do �ady m� s ostatn�mi objekty nekonzistentn� typ.";
		private const string errorMessageBadTypeDetail = "\nTyp: {0}\nTyp �ady: {1}";

		private const string errorMessageBadSize = "Nov� objekt p�id�van� do �ady nem� stejnou velikost jako ostatn� objekty.";
		private const string errorMessageBadSizeDetail = "\nTyp: {0}\nVelikost: {1}\nVelikost v �ad�: {2}";
	}
}
