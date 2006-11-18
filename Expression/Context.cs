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

		// ��dost o ukon�en� programu
		public delegate void ExitEventHandler(object sender, EventArgs e);
		public event ExitEventHandler ExitRequest;

		/// <summary>
		/// Vol� se p�i po�adavku o graf
		/// </summary>
		public void OnGraphRequest(GraphRequestEventArgs e) {
			if(this.GraphRequest != null)
				this.GraphRequest(this, e);
		}

		/// <summary>
		/// Vol� se p�i po�adavku o graf
		/// </summary>
		public void OnExitRequest(EventArgs e) {
			if(this.ExitRequest != null)
				this.ExitRequest(this, e);
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
		public Array ObjectNames() {
			Array retValue = new Array();
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
		/// Prozkoum�, zda se nevyskytuje p��slu�n� HotKey. Pokud ano, spust� jej
		/// </summary>
		/// <param name="key">Znak</param>
		/// <returns>True, pokud byl key nalezen</returns>
		public bool HotKey(char key) {
			foreach(Variable var in this.objects.Values)
				if(var.Item is HotKey && (var.Item as HotKey).Key == key) {
					(var.Item as HotKey).Evaluate();
					return true;
				}
			return false;
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
            // Po�et objekt� k z�znamu
            int num = 0;
            foreach(Variable v in this.objects.Values)
                if(v != null)
                    num++;

            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(num);

                foreach(Variable v in this.objects.Values)
                    if(v != null) {
                        b.Write(v.Name);
                        b.Write(v.Expression);
                        export.Write(v.Item);
                    }
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine(num);

                foreach(Variable v in this.objects.Values)
                    if(v != null) {
                        t.WriteLine(v.Name);
                        t.WriteLine(v.Expression);
                        export.Write(v.Item);
                        t.WriteLine(separator);
                    }
            }
        }

		/// <summary>
		/// Na�te obsah kontextu ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            this.Clear();

            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                int num = b.ReadInt32();

                for(int i = 0; i < num; i++) {
                    string name = b.ReadString();
                    string e = b.ReadString();
                    Assignment assignment = null;

                    if(e != string.Empty) {
                        assignment = new Assignment(this, e, null);
                    }
                    this.SetVariable(name, import.Read(), assignment);
                }
            }
            else {
                // Textov�
                StreamReader t = import.T;
                int num = int.Parse(t.ReadLine());

                for(int i = 0; i < num; i++) {
                    string name = t.ReadLine();
                    string e = t.ReadLine();
                    Assignment assignment = null;

                    if(e != string.Empty)
                        assignment = new Assignment(this, e, null);

                    this.SetVariable(name, import.Read(), assignment);

                    // Pr�zdn� ��dka s odd�lova�em
                    t.ReadLine();
                }
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
