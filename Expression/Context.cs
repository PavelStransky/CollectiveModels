using System;
using System.IO;
using System.Collections;

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

			// M�me - li graf, zobraz�me graf
			if((item is Graph && (item as Graph).Show) || item is GraphArray)
				this.OnGraphRequest(new GraphRequestEventArgs(this, retValue));

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

		#region Implementace IExportable
		/// <summary>
		/// Ulo�� obsah kontextu do souboru
		/// </summary>
		/// <param name="fName">Jm�no souboru</param>
		/// <param name="binary">Ukl�dat v bin�rn� podob�</param>
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
		/// Ulo�� obsah kontextu do souboru textov�
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
		/// Ulo�� obsah kontextu do souboru bin�rn�
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
		/// Na�te obsah kontextu ze souboru
		/// </summary>
		/// <param name="fName">Jm�no souboru</param>
		/// <param name="binary">Soubor v bin�rn� podob�</param>
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
		/// Na�te obsah kontextu ze souboru textov�
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

				// Pr�zdn� ��dka s odd�lova�em
				t.ReadLine();
			}
		}

		/// <summary>
		/// Na�te obsah kontextu ze souboru bin�rn�
		/// </summary>
		/// <param name="b">BinaryReader</param>
		public void Import(BinaryReader b) {
			ImportExportException.CheckImportType(b.ReadString(), this.GetType());

			this.Clear();

			// Kv�li ukon�en� �ten� po dosa�en� konce souboru
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
