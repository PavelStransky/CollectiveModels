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
        private Hashtable objects = new Hashtable();
        private string directory = string.Empty;
        
        private static string fncDirectory = string.Empty;
        private static string globalContextDirectory = string.Empty;

        private static string execDirectory = string.Empty;
        private static string workingDirectory = string.Empty;

        #region Ud�losti - ��dosti pro vn�j�� objekt
        // Zm�na na kontextu
        public delegate void ContextEventHandler(object sender, ContextEventArgs e);
        public event ContextEventHandler ContextEvent;

        /// <summary>
        /// Vol� se p�i jak�koliv ud�losti na kontextu
        /// </summary>
        public void OnEvent(ContextEventArgs e) {
            if(this.ContextEvent != null)
                this.ContextEvent(this, e);
        }
		#endregion

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
		public Context() {}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="directory">Adres��</param>
        public Context(string directory) {
            this.directory = directory;
        }

        /// <summary>
        /// Adres��
        /// </summary>
        public string Directory { get { return this.directory; } set { this.directory = value; } }

        /// <summary>
        /// Adres�� s funkcemi
        /// </summary>
        public static string FncDirectory { get { return fncDirectory; } set { fncDirectory = value; } }

        /// <summary>
        /// Adres�� s glob�ln�m kontextem
        /// </summary>
        public static string GlobalContextDirectory { get { return globalContextDirectory; } set { globalContextDirectory = value; } }

        /// <summary>
        /// Jm�no souboru s glob�ln�m kontextem
        /// </summary>
        public static string GlobalContextFileName { get { return Path.Combine(globalContextDirectory, globalContextFileName); } }

        /// <summary>
        /// Directory with the main executable
        /// </summary>
        public static string ExecDirectory { get { return execDirectory; } set { execDirectory = value; } }

        /// <summary>
        /// Working directory
        /// </summary>
        public static string WorkingDirectory { get { return workingDirectory; } set { workingDirectory = value; } }

        /// <summary>
        /// P�id� syst�movou prom�nnou do kontextu (nemus� m�t platn� n�zev). Pokud u� existuje, nahrad� ji
        /// </summary>
        /// <param name="name">Jm�no prom�nn�</param>
        /// <param name="item">Hodnota prom�nn�</param>
        public Variable SetSystemVariable(string name, object item) {
            Variable retValue = null;

            if(this.objects.ContainsKey(name)) {
                // Pokud u� prom�nn� na kontextu existuje, zm�n�me pouze jej� hodnotu
                retValue = this[name];
                retValue.Item = item;
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

            else {
                // Jinak ji mus�me vytvo�it
                this.objects.Add(name, retValue = new Variable(name, item, false));
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

            return retValue;
        }
        
        /// <summary>
		/// P�id� prom�nnou do kontextu. Pokud u� existuje, nahrad� ji
		/// </summary>
		/// <param name="name">Jm�no prom�nn�</param>
		/// <param name="item">Hodnota prom�nn�</param>
		public Variable SetVariable(string name, object item) {
			Variable retValue = null;

            if(name == directoryVariable) {
                this.directory = (string)item;
                this.OnEvent(new ContextEventArgs(ContextEventType.ChangeDirectory, this.directory));
            }

            else if(name == fncDirectoryVariable)
                fncDirectory = (string)item;

            else if(name == globalContextDirectoryVariable)
                globalContextDirectory = (string)item;

            else if(name == execDirectoryVariable)
                execDirectory = (string)item;

            else if(name == workingDirectoryVariable)
                workingDirectory = (string)item;

            else if(name == piVariable)
                throw new ContextException(string.Format(Messages.EMVarCannotBeSet, piVariable));

            else if(this.objects.ContainsKey(name)) {
                // Pokud u� prom�nn� na kontextu existuje, zm�n�me pouze jej� hodnotu
                retValue = this[name];
                retValue.Item = item;
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

            else {
                // Jinak ji mus�me vytvo�it
                this.objects.Add(name, retValue = new Variable(name, item));
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

			return retValue;
		}

		/// <summary>
		/// Vyma�e v�e, co je v kontextu ulo�eno
		/// </summary>
		public void Clear() {
			this.objects.Clear();
            this.OnEvent(new ContextEventArgs(ContextEventType.Change));
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

            this.OnEvent(new ContextEventArgs(ContextEventType.Change));
        }

		/// <summary>
		/// Jm�na v�ech objekt� na kontextu
		/// </summary>
		public TArray ObjectNames() {
			TArray result = new TArray(typeof(string), this.objects.Count);
            int i = 0;
			foreach(string key in this.objects.Keys)
                result[i++] = key;

            return result;
		}

		/// <summary>
		/// Vyhled� a vr�t� prom�nnou
		/// </summary>
		/// <param name="name">Jm�no prom�nn�</param>
		public Variable this[string name] {
			get {
                if(name == directoryVariable)
                    return new Variable(name, this.directory);

                else if(name == fncDirectoryVariable)
                    return new Variable(name, fncDirectory);

                else if(name == globalContextDirectoryVariable)
                    return new Variable(name, globalContextDirectory);

                else if(name == execDirectoryVariable)
                    return new Variable(name, execDirectory);

                else if(name == workingDirectoryVariable)
                    return new Variable(name, workingDirectory);

                else if(name == piVariable)
                    return new Variable(name, System.Math.PI);

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

                object item = this[key].Item;
                s.Append(item.GetType().FullName);

                if(item is Vector)
                    s.Append(string.Format(" [{0}]", (item as Vector).Length));
                else if(item is PointVector)
                    s.Append(string.Format(" [{0}]", (item as PointVector).Length));
                else if(item is List)
                    s.Append(string.Format(" [{0}]", (item as List).Count));
                else if(item is TArray) {
                    s.Append(string.Format(" [{0}] ", (item as TArray).LengthsString()));
                    s.Append((item as TArray).GetItemType());
                }

                s.Append(')');
                s.Append(Environment.NewLine);
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
                    param.Add(v.Item, v.Name, null);
                }

            param.Add(directory, directoryVariable, null);
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
                if(name == directoryVariable)
                    this.directory = o as string;
                else
                    this.SetVariable(name, o);
            }
        }
		#endregion

		private const string errorMessageNoObject = "Objekt \"{0}\" nebyl nalezen.";
		private const string separator = "-----------------------------------------------------------";

        private const string directoryVariable = "_dir";
        private const string fncDirectoryVariable = "_fncdir";
        private const string globalContextDirectoryVariable = "_gcdir";
        private const string execDirectoryVariable = "_execdir";
        private const string workingDirectoryVariable = "_workingdir";
        private const string piVariable = "_pi";
        private const string globalContextFileName = "global.ctx";
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