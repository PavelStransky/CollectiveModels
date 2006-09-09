using System;
using System.Collections;
using System.Windows.Forms;

namespace PavelStransky.Controls {
	/// <summary>
	/// Kontext, ve kterém jsou uloženy všechny formuláøe
	/// </summary>
	public class Context {
		private Hashtable forms;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public Context() {
			this.forms = new Hashtable();
		}

		/// <summary>
		/// Pøidá formuláø do kontextu. Pokud už existuje, jen jej zobrazí
		/// </summary>
		/// <param name="form">Formuláø, který bude pøidán do kontextu</param>
		public void Add(ContextForm form) {
			if(this.forms.ContainsKey(form.Name)) 
				throw new ContextException(string.Format(errorMessageFormExists, form.Name));
			else 
				this.forms.Add(form.Name, form);
		}

		/// <summary>
		/// Vymaže vše, co je v kontextu uloženo
		/// </summary>
        /// <returns>True, pokud má být uzavírání ukonèeno</returns>
		public bool CloseAll() {
			ArrayList names = new ArrayList();
            foreach(string name in this.forms.Keys)
                names.Add(name);

            foreach(string name in names)
                if(this.Close(name))
                    return true;

            return false;
		}

		/// <summary>
		/// Zavøe formuláø daného názvu
		/// </summary>
		/// <param name="name">Název formuláøe</param>
        /// <returns>True, pokud má být uzavírání ukonèeno</returns>
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

            return false;
		}

		/// <summary>
		/// Vymaže formuláø z kontextu (pro pøípad, že Close provádí sám formuláø)
		/// </summary>
		/// <param name="name">Název formuláøe</param>
		public void Clear(string name) {
			if(this.forms.ContainsKey(name))
				this.forms.Remove(name);
			else
				throw new ContextException(string.Format(errorMessageNoObject, name));
		}

		/// <summary>
		/// Vyhledá a vrátí formuláø
		/// </summary>
		/// <param name="name">Jméno formuláøe</param>
		public ContextForm this[string name] {
			get {
				ContextForm form = this.forms[name] as ContextForm;
				if(form == null)
					throw new ContextException(string.Format(errorMessageNoObject, name));

				return form;
			}
		}

		/// <summary>
		/// Zjistí, zda na kontextu existuje objekt s daným názvem
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name) {
			return this.forms.ContainsKey(name);
		}

		/// <summary>
		/// Vytvoøí (nebo najde a vrátí) formuláø na kontextu
		/// </summary>
		/// <param name="name">Jméno formuláøe</param>
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

		private const string errorMessageNoObject = "Formuláø \"{0}\" nebyl nalezen.";
		private const string errorMessageFormExists = "Formuláø \"{0}\" již existuje. Nelze jej znovu pøidat na kontext.";
		private const string errorMessageUnknownType = "Nelze vytvoøit formuláø se \"{0}\". Požadován neznámý typ {1}.";
	}

	/// <summary>
	/// Výjimka ve tøídì Context
	/// </summary>
	public class ContextException: ApplicationException {
		private string detailMessage = string.Empty;

		/// <summary>
		/// Pøídavné informace o výjimce
		/// </summary>
		public string DetailMessage {get {return this.detailMessage;}}

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
		public ContextException(string message, string detailMessage) : this(message) {
			this.detailMessage = detailMessage;
		}

		private const string errMessage = "Na kontextu došlo k chybì: ";
	}
}
