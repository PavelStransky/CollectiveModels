using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Kontext, ve kterém jsou uloženy všechny promìnné
	/// </summary>
	public class Context: IExportable {
		#region Události - žádosti pro vnìjší objekt
		// Žádost o graf
		public delegate void GraphRequestEventHandler(object sender, GraphRequestEventArgs e);
		public event GraphRequestEventHandler GraphRequest;

        /// <summary>
        /// Volá se pøi požadavku o graf
        /// </summary>
        public void OnGraphRequest(GraphRequestEventArgs e) {
            if(this.GraphRequest != null)
                this.GraphRequest(this, e);
        }

		// Žádost o ukonèení programu
		public delegate void ExitEventHandler(object sender, EventArgs e);
		public event ExitEventHandler ExitRequest;

		/// <summary>
		/// Volá se pøi požadavku o konec programu
		/// </summary>
		public void OnExitRequest(EventArgs e) {
			if(this.ExitRequest != null)
				this.ExitRequest(this, e);
		}

        // Žádost o uložení souboru
        public delegate void FileNameEventHandler(object sender, FileNameEventArgs e);
        public event FileNameEventHandler SaveRequest;

        /// <summary>
        /// Volá se pøi požadavku o uložení
        /// </summary>
        public void OnSaveRequest(FileNameEventArgs e) {
            if(this.SaveRequest != null)
                this.SaveRequest(this, e);
        }

        // Vytvoøení kontextu
        public delegate void ContextEventHandler(object sender, ContextEventArgs e);
        public event ContextEventHandler NewContextRequest;

        /// <summary>
        /// Volá se pøi vytvoøení nového kontextu
        /// </summary>
        public void OnNewContextRequest(ContextEventArgs e) {
            if(this.NewContextRequest != null)
                this.NewContextRequest(this, e);
        }

        // Žádost o zmìnu kontextu
        public event ContextEventHandler SetContextRequest;

        /// <summary>
        /// Volá se pøi žádosti o zmìnu kontextu
        /// </summary>
        public void OnSetContextRequest(ContextEventArgs e) {
            if(this.SetContextRequest != null)
                this.SetContextRequest(this, e);
        }
		#endregion

		private Hashtable objects;

		public Context() {
			this.objects = new Hashtable();
		}

		/// <summary>
		/// Pøidá promìnnou do kontextu. Pokud už existuje, nahradí ji
		/// </summary>
		/// <param name="name">Jméno promìnné</param>
		/// <param name="item">Hodnota promìnné</param>
		/// <param name="assignment">Výraz pro pøiøazení</param>
		public Variable SetVariable(string name, object item, Assignment assignment) {
			Variable retValue = null;

			if(this.objects.ContainsKey(name)) { 
				// Pokud už promìnná na kontextu existuje, zmìníme pouze její hodnotu
				retValue = this[name];
				retValue.Item = item;
				retValue.Assignment = assignment;
			}
			else 
				// Jinak ji musíme vytvoøit
				this.objects.Add(name, retValue = new Variable(this, name, item, assignment));

			return retValue;
		}

		/// <summary>
		/// Pøidá promìnnou do kontextu. Pokud už existuje, nahradí ji
		/// </summary>
		/// <param name="name">Jméno promìnné</param>
		/// <param name="item">Hodnota promìnné</param>
		public Variable SetVariable(string name, object item) {
			return this.SetVariable(name, item, null);
		}

		/// <summary>
		/// Vymaže vše, co je v kontextu uloženo
		/// </summary>
		public void Clear() {
			this.objects.Clear();
		}

		/// <summary>
		/// Vymaže promìnnou daného názvu
		/// </summary>
		/// <param name="name">Název promìnné</param>
		public void Clear(string name) {
			if(this.objects.ContainsKey(name))
				this.objects.Remove(name);
			else
				throw new ContextException(string.Format(errorMessageNoObject, name));
		}

		/// <summary>
		/// Jména všech objektù na kontextu
		/// </summary>
		public TArray ObjectNames() {
			TArray retValue = new TArray();
			foreach(string key in this.objects.Keys)
				retValue.Add(key);
			return retValue;
		}

		/// <summary>
		/// Vyhledá a vrátí promìnnou
		/// </summary>
		/// <param name="name">Jméno promìnné</param>
		public Variable this[string name] {
			get {
				Variable variable = this.objects[name] as Variable;
				if(variable == null)
					throw new ContextException(string.Format(errorMessageNoObject, name));

				return variable;
			}
		}

		/// <summary>
		/// Zjistí, zda na kontextu existuje objekt s daným názvem
		/// </summary>
		/// <param name="name">Název objektu</param>
		public bool Contains(string name) {
			return this.objects.ContainsKey(name);
		}

        /// <summary>
        /// Vypíše názvy a typy všech promìnných na kontextu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();

            foreach(string key in this.objects.Keys) {
                s.Append(key);
                s.Append(" (");
                s.Append(this[key].Item.GetType().FullName);
                s.Append(")\n");
            }

            return s.ToString();
        }

		#region Implementace IExportable
		/// <summary>
		/// Uloží obsah kontextu do souboru
		/// </summary>
		/// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            foreach(Variable v in this.objects.Values)
                if(v != null) {
                    param.Add(v.Item, v.Name, v.Expression);
                }

            param.Export(export);
        }

		/// <summary>
		/// Naète obsah kontextu ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            this.Clear();

            IEParam param = new IEParam(import);

            int count = param.Count;
            for(int i = 0; i < count; i++) {
                string name, expression;
                object o = param.Get(null, out name, out expression);
                this.SetVariable(name, o, expression != string.Empty ? new Assignment(expression, null) : null);
            }
        }
		#endregion

		private const string errorMessageNoObject = "Objekt \"{0}\" nebyl nalezen.";
		private const string separator = "-----------------------------------------------------------";
	}

	/// <summary>
	/// Výjimka ve tøídì Context
	/// </summary>
	public class ContextException: DetailException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ContextException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ContextException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		/// <param name="detailMessage">Detail chyby</param>
		public ContextException(string message, string detailMessage) : base(errMessage + message, detailMessage) {}

		private const string errMessage = "Na kontextu došlo k chybì: ";
	}
}
