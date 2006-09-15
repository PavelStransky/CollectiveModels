using System;
using System.Collections;
using System.Windows.Forms;

namespace PavelStransky.Forms {
	/// <summary>
	/// Kontext, ve kter�m jsou ulo�eny v�echny formul��e
	/// </summary>
	public class Context {
		private Hashtable forms;

        // Zm�na v kontextu
        public delegate void ChangeEventHandler(object sender, EventArgs e);
        public event ChangeEventHandler Change;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public Context() {
			this.forms = new Hashtable();
		}
        
		/// <summary>
		/// P�id� formul�� do kontextu. Pokud u� existuje, jen jej zobraz�
		/// </summary>
		/// <param name="form">Formul��, kter� bude p�id�n do kontextu</param>
		public void Add(ContextForm form) {
			if(this.forms.ContainsKey(form.Name)) 
				throw new ContextException(string.Format(errorMessageFormExists, form.Name));
			else 
				this.forms.Add(form.Name, form);

            this.OnChange(new EventArgs());
		}

        /// <summary>
        /// Vol�no p�i p�id�n� nov�ho formul��e do kontextu
        /// </summary>
        protected void OnChange(EventArgs e) {
            if(this.Change != null)
                this.Change(this, e);
        }

		/// <summary>
		/// Vyma�e v�e, co je v kontextu ulo�eno
		/// </summary>
        /// <returns>True, pokud m� b�t uzav�r�n� ukon�eno</returns>
		public bool CloseAll() {
			ArrayList names = new ArrayList();
            foreach(string name in this.forms.Keys)
                names.Add(name);

            foreach(string name in names)
                if(this.Close(name)) 
                    return true;

            this.OnChange(new EventArgs());
            return false;
		}

		/// <summary>
		/// Zav�e formul�� dan�ho n�zvu
		/// </summary>
		/// <param name="name">N�zev formul��e</param>
        /// <returns>True, pokud m� b�t uzav�r�n� ukon�eno</returns>
        public bool Close(string name) {
			if(this.forms.ContainsKey(name)) {
				ContextForm form = this.forms[name] as ContextForm;
                if(form.Close())
                    return true;
                if(this.forms.ContainsKey(name))
                    this.Clear(name);
			}
			else
				throw new ContextException(string.Format(errorMessageNoObject, name));

            this.OnChange(new EventArgs());
            return false;
		}

		/// <summary>
		/// Vyma�e formul�� z kontextu (pro p��pad, �e Close prov�d� s�m formul��)
		/// </summary>
		/// <param name="name">N�zev formul��e</param>
		public void Clear(string name) {
			if(this.forms.ContainsKey(name))
				this.forms.Remove(name);
			else
				throw new ContextException(string.Format(errorMessageNoObject, name));
		}

		/// <summary>
		/// Vyhled� a vr�t� formul��
		/// </summary>
		/// <param name="name">Jm�no formul��e</param>
		public ContextForm this[string name] {
			get {
				ContextForm form = this.forms[name] as ContextForm;
				if(form == null)
					throw new ContextException(string.Format(errorMessageNoObject, name));

				return form;
			}
		}

        /// <summary>
        /// Jm�na v�ech formul��� na kontextu
        /// </summary>
        public string [] ObjectNames() {
            string [] retValue = new string[this.forms.Keys.Count];
            int i = 0;
            foreach(string key in this.forms.Keys)
                retValue[i++] = key;
            return retValue;
        }

		/// <summary>
		/// Zjist�, zda na kontextu existuje objekt s dan�m n�zvem
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name) {
			return this.forms.ContainsKey(name);
		}

		/// <summary>
		/// Vytvo�� (nebo najde a vr�t�) formul�� na kontextu
		/// </summary>
		/// <param name="name">Jm�no formul��e</param>
		public ContextForm CreateForm(Type formType, string name) {
			if(this.forms.ContainsKey(name))
				return this.forms[name] as ContextForm;
			else {
				ContextForm form = null;
                if(formType == typeof(GraphForm))
                    form = new GraphForm(this);
                else if(formType == typeof(ResultForm))
                    form = new ResultForm(this);
                else
                    throw new ContextException(string.Format(errorMessageUnknownType, name, formType.FullName));

				form.Name = name;
				form.Text = name;

				this.Add(form);
				return form;
    		}
		}

		private const string errorMessageNoObject = "Formul�� \"{0}\" nebyl nalezen.";
		private const string errorMessageFormExists = "Formul�� \"{0}\" ji� existuje. Nelze jej znovu p�idat na kontext.";
		private const string errorMessageUnknownType = "Nelze vytvo�it formul�� se \"{0}\". Po�adov�n nezn�m� typ {1}.";
	}

	/// <summary>
	/// V�jimka ve t��d� Context
	/// </summary>
	public class ContextException: ApplicationException {
		private string detailMessage = string.Empty;

		/// <summary>
		/// P��davn� informace o v�jimce
		/// </summary>
		public string DetailMessage {get {return this.detailMessage;}}

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
		public ContextException(string message, string detailMessage) : this(message) {
			this.detailMessage = detailMessage;
		}

		private const string errMessage = "Na kontextu do�lo k chyb�: ";
	}
}
