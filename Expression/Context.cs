using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Kontext, ve kter�m jsou ulo�eny v�echny prom�nn�
	/// </summary>
	public class Context: IExportable {
		#region Ud�losti - ��dosti pro vn�j�� objekt
		// ��dost o graf
		public delegate void GraphRequestEventHandler(object sender, GraphRequestEventArgs e);
		public event GraphRequestEventHandler GraphRequest;

        /// <summary>
        /// Vol� se p�i po�adavku o graf
        /// </summary>
        public void OnGraphRequest(GraphRequestEventArgs e) {
            if(this.GraphRequest != null)
                this.GraphRequest(this, e);
        }

		// ��dost o ukon�en� programu
		public delegate void ExitEventHandler(object sender, EventArgs e);
		public event ExitEventHandler ExitRequest;

		/// <summary>
		/// Vol� se p�i po�adavku o konec programu
		/// </summary>
		public void OnExitRequest(EventArgs e) {
			if(this.ExitRequest != null)
				this.ExitRequest(this, e);
		}

        // ��dost o ulo�en� souboru
        public delegate void FileNameEventHandler(object sender, FileNameEventArgs e);
        public event FileNameEventHandler SaveRequest;

        /// <summary>
        /// Vol� se p�i po�adavku o ulo�en�
        /// </summary>
        public void OnSaveRequest(FileNameEventArgs e) {
            if(this.SaveRequest != null)
                this.SaveRequest(this, e);
        }

        // Vytvo�en� kontextu
        public delegate void ContextEventHandler(object sender, ContextEventArgs e);
        public event ContextEventHandler NewContextRequest;

        /// <summary>
        /// Vol� se p�i vytvo�en� nov�ho kontextu
        /// </summary>
        public void OnNewContextRequest(ContextEventArgs e) {
            if(this.NewContextRequest != null)
                this.NewContextRequest(this, e);
        }

        // ��dost o zm�nu kontextu
        public event ContextEventHandler SetContextRequest;

        /// <summary>
        /// Vol� se p�i ��dosti o zm�nu kontextu
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
		/// P�id� prom�nnou do kontextu. Pokud u� existuje, nahrad� ji
		/// </summary>
		/// <param name="name">Jm�no prom�nn�</param>
		/// <param name="item">Hodnota prom�nn�</param>
		/// <param name="assignment">V�raz pro p�i�azen�</param>
		public Variable SetVariable(string name, object item, Assignment assignment) {
			Variable retValue = null;

			if(this.objects.ContainsKey(name)) { 
				// Pokud u� prom�nn� na kontextu existuje, zm�n�me pouze jej� hodnotu
				retValue = this[name];
				retValue.Item = item;
				retValue.Assignment = assignment;
			}
			else 
				// Jinak ji mus�me vytvo�it
				this.objects.Add(name, retValue = new Variable(this, name, item, assignment));

			return retValue;
		}

		/// <summary>
		/// P�id� prom�nnou do kontextu. Pokud u� existuje, nahrad� ji
		/// </summary>
		/// <param name="name">Jm�no prom�nn�</param>
		/// <param name="item">Hodnota prom�nn�</param>
		public Variable SetVariable(string name, object item) {
			return this.SetVariable(name, item, null);
		}

		/// <summary>
		/// Vyma�e v�e, co je v kontextu ulo�eno
		/// </summary>
		public void Clear() {
			this.objects.Clear();
		}

		/// <summary>
		/// Vyma�e prom�nnou dan�ho n�zvu
		/// </summary>
		/// <param name="name">N�zev prom�nn�</param>
		public void Clear(string name) {
			if(this.objects.ContainsKey(name))
				this.objects.Remove(name);
			else
				throw new ContextException(string.Format(errorMessageNoObject, name));
		}

		/// <summary>
		/// Jm�na v�ech objekt� na kontextu
		/// </summary>
		public TArray ObjectNames() {
			TArray retValue = new TArray();
			foreach(string key in this.objects.Keys)
				retValue.Add(key);
			return retValue;
		}

		/// <summary>
		/// Vyhled� a vr�t� prom�nnou
		/// </summary>
		/// <param name="name">Jm�no prom�nn�</param>
		public Variable this[string name] {
			get {
				Variable variable = this.objects[name] as Variable;
				if(variable == null)
					throw new ContextException(string.Format(errorMessageNoObject, name));

				return variable;
			}
		}

		/// <summary>
		/// Zjist�, zda na kontextu existuje objekt s dan�m n�zvem
		/// </summary>
		/// <param name="name">N�zev objektu</param>
		public bool Contains(string name) {
			return this.objects.ContainsKey(name);
		}

        /// <summary>
        /// Vyp�e n�zvy a typy v�ech prom�nn�ch na kontextu
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
		/// Ulo�� obsah kontextu do souboru
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
		/// Na�te obsah kontextu ze souboru
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
	/// V�jimka ve t��d� Context
	/// </summary>
	public class ContextException: DetailException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ContextException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ContextException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		/// <param name="detailMessage">Detail chyby</param>
		public ContextException(string message, string detailMessage) : base(errMessage + message, detailMessage) {}

		private const string errMessage = "Na kontextu do�lo k chyb�: ";
	}
}
