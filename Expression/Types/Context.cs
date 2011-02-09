using System;
using System.IO;
using System.Collections;
using System.Text;

using System.Threading;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
	/// Kontext, ve kterém jsou uloženy všechny promìnné
	/// </summary>
	public class Context: IExportable {
        private Hashtable objects = new Hashtable();
        private string directory = string.Empty;

        private Mutex contextMutex = new Mutex(false, "Context");

        private static string fncDirectory = string.Empty;
        private static string globalContextDirectory = string.Empty;

        private static string execDirectory = string.Empty;
        private static string workingDirectory = string.Empty;

        #region Události - žádosti pro vnìjší objekt
        // Zmìna na kontextu
        public delegate void ContextEventHandler(object sender, ContextEventArgs e);
        public event ContextEventHandler ContextEvent;

        /// <summary>
        /// Volá se pøi jakékoliv události na kontextu
        /// </summary>
        public void OnEvent(ContextEventArgs e) {
            if(this.ContextEvent != null)
                this.ContextEvent(this, e);
        }
		#endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="directory">Adresáø</param>
        public Context(string directory) {
            this.directory = directory;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Context() { }

        /// <summary>
        /// Adresáø
        /// </summary>
        public string Directory { get { return this.directory; } set { this.directory = value; } }

        /// <summary>
        /// Adresáø s funkcemi
        /// </summary>
        public static string FncDirectory { get { return fncDirectory; } set { fncDirectory = value; } }

        /// <summary>
        /// Adresáø s globálním kontextem
        /// </summary>
        public static string GlobalContextDirectory { get { return globalContextDirectory; } set { globalContextDirectory = value; } }

        /// <summary>
        /// Jméno souboru s globálním kontextem
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
        /// Pøidá systémovou promìnnou do kontextu (nemusí mít platný název). Pokud už existuje, nahradí ji
        /// </summary>
        /// <param name="name">Jméno promìnné</param>
        /// <param name="item">Hodnota promìnné</param>
        public Variable SetSystemVariable(string name, object item) {
            Variable retValue = null;

            if(this.objects.ContainsKey(name)) {
                // Pokud už promìnná na kontextu existuje, zmìníme pouze její hodnotu
                retValue = this[name];
                retValue.Item = item;
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

            else {
                // Jinak ji musíme vytvoøit
                this.objects.Add(name, retValue = new Variable(name, item, false));
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

            return retValue;
        }
        
        /// <summary>
		/// Pøidá promìnnou do kontextu. Pokud už existuje, nahradí ji
		/// </summary>
		/// <param name="name">Jméno promìnné</param>
		/// <param name="item">Hodnota promìnné</param>
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
                // Pokud už promìnná na kontextu existuje, zmìníme pouze její hodnotu
                retValue = this[name];
                this.contextMutex.WaitOne();
                retValue.Item = item;
                this.contextMutex.ReleaseMutex();
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

            else {
                // Jinak ji musíme vytvoøit
                this.contextMutex.WaitOne();
                this.objects.Add(name, retValue = new Variable(name, item));
                this.contextMutex.ReleaseMutex();
                this.OnEvent(new ContextEventArgs(ContextEventType.Change));
            }

			return retValue;
		}

		/// <summary>
		/// Vymaže vše, co je v kontextu uloženo
		/// </summary>
		public void Clear() {
            this.contextMutex.WaitOne();
			this.objects.Clear();
            this.contextMutex.ReleaseMutex();
            this.OnEvent(new ContextEventArgs(ContextEventType.Change));
        }

		/// <summary>
		/// Vymaže promìnnou daného názvu
		/// </summary>
		/// <param name="name">Název promìnné</param>
		public void Clear(string name) {
            if(this.objects.ContainsKey(name)) {
                this.contextMutex.WaitOne();
                this.objects.Remove(name);
                this.contextMutex.ReleaseMutex();
            }
            else
                throw new ContextException(string.Format(Messages.EMNoVariable, name));

            this.OnEvent(new ContextEventArgs(ContextEventType.Change));
        }

		/// <summary>
		/// Jména všech objektù na kontextu
		/// </summary>
		public TArray ObjectNames() {
			TArray result = new TArray(typeof(string), this.objects.Count);
            int i = 0;
			foreach(string key in this.objects.Keys)
                result[i++] = key;

            return result;
		}

		/// <summary>
		/// Vyhledá a vrátí promìnnou
		/// </summary>
		/// <param name="name">Jméno promìnné</param>
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
					throw new ContextException(string.Format(Messages.EMNoVariable, name));

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
		/// Uloží obsah kontextu do souboru
		/// </summary>
		/// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            this.contextMutex.WaitOne();
            foreach(Variable v in this.objects.Values)
                if(v != null && v.Name[0] != '$') {
                    param.Add(v.Item, v.Name, null);
                }
            this.contextMutex.ReleaseMutex();

            param.Add(directory, directoryVariable, null);
            param.Export(export);
        }

		/// <summary>
		/// Naète obsah kontextu ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public Context(Core.Import import) {
            IEParam param = new IEParam(import);

            this.contextMutex.WaitOne();
            int count = param.Count;
            for(int i = 0; i < count; i++) {
                string name, expression;
                object o = param.Get(null, out name, out expression);
                if(name == directoryVariable)
                    this.directory = o as string;
                else
                    this.SetVariable(name, o);
            }
            this.contextMutex.ReleaseMutex();
        }
		#endregion

		private const string separator = "-----------------------------------------------------------";

        private const string directoryVariable = "_dir";
        private const string fncDirectoryVariable = "_fncdir";
        private const string globalContextDirectoryVariable = "_gcdir";
        private const string execDirectoryVariable = "_execdir";
        private const string workingDirectoryVariable = "_workingdir";
        private const string piVariable = "_pi";
        private const string globalContextFileName = "global.ctx";
    }
}
