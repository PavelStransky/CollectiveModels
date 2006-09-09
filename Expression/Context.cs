using System;
using System.IO;
using System.Collections;

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

		// Žádost o ukonèení programu
		public delegate void ExitEventHandler(object sender, EventArgs e);
		public event ExitEventHandler ExitRequest;

		/// <summary>
		/// Volá se pøi požadavku o graf
		/// </summary>
		public void OnGraphRequest(GraphRequestEventArgs e) {
			if(this.GraphRequest != null)
				this.GraphRequest(this, e);
		}

		/// <summary>
		/// Volá se pøi požadavku o graf
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

			// Máme - li graf, zobrazíme graf
			if((item is Graph && (item as Graph).Show) || item is GraphArray)
				this.OnGraphRequest(new GraphRequestEventArgs(this, retValue));

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
		public Array ObjectNames() {
			Array retValue = new Array();
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
		/// Prozkoumá, zda se nevyskytuje pøíslušný HotKey. Pokud ano, spustí jej
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

		#region Implementace IExportable
		/// <summary>
		/// Uloží obsah kontextu do souboru
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
		/// Uloží obsah kontextu do souboru textovì
		/// </summary>
		/// <param name="t">StreamWriter</param>
		public void Export(StreamWriter t) {
			t.WriteLine(this.GetType().FullName);

			foreach(Variable v in this.objects.Values) {
				if(v != null) {
					t.WriteLine(v.Name);
					t.WriteLine(v.Item.GetType().FullName);
					t.WriteLine(v.Expression);
					IExportable iExportable = v.Item as IExportable;
					if(iExportable != null)
						iExportable.Export(t);
					else if(v.Item is double || v.Item is int || v.Item is string)
						t.WriteLine(v.Item);
					else if(v.Item is PointD) {
						t.WriteLine("{0}\t{1}", (v.Item as PointD).X, (v.Item as PointD).Y);
					}
					t.WriteLine(separator);
				}
			}		
		}

		/// <summary>
		/// Uloží obsah kontextu do souboru binárnì
		/// </summary>
		/// <param name="b">BinaryWriter</param>
		public void Export(BinaryWriter b) {
			b.Write(this.GetType().FullName);

			foreach(Variable v in this.objects.Values) {
				if(v != null) {
					b.Write(v.Name);
					b.Write(v.Item.GetType().FullName);
					b.Write(v.Expression);
					IExportable iExportable = v.Item as IExportable;
					if(iExportable != null)
						iExportable.Export(b);
					else if(v.Item is double) 
						b.Write((double)v.Item);
					else if(v.Item is int)
						b.Write((int)v.Item);
					else if(v.Item is string)
						b.Write((string)v.Item);
					else if(v.Item is PointD) {
						b.Write((v.Item as PointD).X);
						b.Write((v.Item as PointD).Y);
					}
				}
			}		
		}

		/// <summary>
		/// Naète obsah kontextu ze souboru
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
		/// Naète obsah kontextu ze souboru textovì
		/// </summary>
		/// <param name="t">StreamReader</param>
		public void Import(StreamReader t) {
			ImportExportException.CheckImportType(t.ReadLine(), this.GetType());			

			this.Clear();

			string name;
			while((name = t.ReadLine()) != null) {
				string typeName = t.ReadLine();
				string e = t.ReadLine();
				Assignment assignment = null;

				if(e != string.Empty)
					assignment = new Assignment(this, e, null);

				if(typeName == typeof(Vector).FullName) 
					this.SetVariable(name, new Vector(t), assignment);
				else if(typeName == typeof(PointVector).FullName)
					this.SetVariable(name, new PointVector(t), assignment);
				else if(typeName == typeof(Matrix).FullName) 
					this.SetVariable(name, new Matrix(t), assignment);
				else if(typeName == typeof(Array).FullName) 
					this.SetVariable(name, new Array(t), assignment);
				else if(typeName == typeof(double).FullName)
					this.SetVariable(name, double.Parse(t.ReadLine()), assignment);
				else if(typeName == typeof(int).FullName)
					this.SetVariable(name, int.Parse(t.ReadLine()), assignment);
				else if(typeName == typeof(PointD).FullName) {
					string line = t.ReadLine();
					string []s = line.Split('\t');
					this.SetVariable(name, new PointD(double.Parse(s[0]), double.Parse(s[1])), assignment);
				}

				// Prázdná øádka s oddìlovaèem
				t.ReadLine();
			}
		}

		/// <summary>
		/// Naète obsah kontextu ze souboru binárnì
		/// </summary>
		/// <param name="b">BinaryReader</param>
		public void Import(BinaryReader b) {
			ImportExportException.CheckImportType(b.ReadString(), this.GetType());

			this.Clear();

			// Kvùli ukonèení ètení po dosažení konce souboru
			try {
				while(true) {
					string name = b.ReadString();
					string typeName = b.ReadString();
					string e = b.ReadString();
					Assignment assignment = null;

					if(e != string.Empty)
						assignment = new Assignment(this, e, null);

					if(typeName == typeof(Vector).FullName) 
						this.SetVariable(name, new Vector(b), assignment);
					else if(typeName == typeof(PointVector).FullName) 
						this.SetVariable(name, new PointVector(b), assignment);
					else if(typeName == typeof(Matrix).FullName) 
						this.SetVariable(name, new Matrix(b), assignment);
					else if(typeName == typeof(Array).FullName) 
						this.SetVariable(name, new Array(b), assignment);
					else if(typeName == typeof(double).FullName)
						this.SetVariable(name, b.ReadDouble(), assignment);
					else if(typeName == typeof(int).FullName)
						this.SetVariable(name, b.ReadInt32(), assignment);
					else if(typeName == typeof(string).FullName)
						this.SetVariable(name, b.ReadString(), assignment);
					else if(typeName == typeof(PointD).FullName)
						this.SetVariable(name, new PointD(b.ReadDouble(), b.ReadDouble()), assignment);
				}
			}
			catch(EndOfStreamException) {}
			catch(Exception e) {
				throw e;
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
