using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms
{
	/// <summary>
	/// Historie p��kaz�
	/// </summary>
	public class History: ArrayList
	{
		// Index aktu�ln�ho z�znamu
		private int index = 0;
		// Aktu�ln� kontext
		private Context context;

		/// <summary>
		/// Index aktu�ln�ho z�znamu
		/// </summary>
		public int Index {get {return this.index;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="context">Aktu�ln� kontext ke zpracov�n�</param>
		public History(Context context): base() {
			this.context = context;
		}

		/// <summary>
		/// P�id� �et�zec s obsahem p��kazu do �ady
		/// </summary>
		/// <param name="value">�et�zec p��kazu</param>
		/// <returns>Index nov�ho z�znamu</returns>
		public override int Add(object value) {
			this.index = 0;
			return base.Add (value);
		}

		/// <summary>
		/// Vynulov�n� historie (mus�me vynulovat i index)
		/// </summary>
		public override void Clear() {
			this.index = -1;
			base.Clear ();
		}

		/// <summary>
		/// Vr�t� ��dky v�echny ��dky, ka�dou ve vlastn�m stringu
		/// </summary>
		public string [] GetAllLines() {
			string [] lines = new string[this.Count];
			for(int i = 0; i < this.Count; i++)
				lines[i] = this[i] as string;
			return lines;
		}

		/// <summary>
		/// Nastav� v�echny ��dky podle vstupu
		/// </summary>
		public void SetAllLines(string [] lines) {
			this.Clear();

			for(int i = 0; i < lines.Length; i++)
				this.Add(lines[i]);
		}

		/// <summary>
		/// P�e�te ��dku na aktu�ln�m indexu a aktu�ln� index posune zp�t
		/// </summary>
		public string GetPreviousHistoryLine() {
			if(this.Count != 0) {
				this.index += this.Count;
				this.index--;
				this.index %= this.Count;
				return this[this.index] as string;
			}
			return null;
		}

		/// <summary>
		/// P�e�te ��dku na aktu�ln�m indexu a aktu�ln� index posune vp�ed
		/// </summary>
		public string GetNextHistoryLine() {
			if(this.Count != 0) {
				this.index++;
				this.index %= this.Count;
				return this[this.index] as string;
			}
			return null;
		}

		/// <summary>
		/// Spust� v�echny p��kazy v historii
		/// </summary>
		public void Evaluate() {
			if(this.Count == 0)
				return;

			for(int i = 0; i < this.Count; i++) {
				Expression.Expression expression = new PavelStransky.Expression.Expression(this.context, this[i] as string);
				expression.Evaluate();
			}
		}

		/// <summary>
		/// Ulo�� obsah historie do souboru
		/// </summary>
		/// <param name="fName">Jm�no souboru</param>
		public void Export(string fName) {
			FileStream f = new FileStream(fName, FileMode.Create);
			StreamWriter t = new StreamWriter(f);

			this.Export(t);

			t.Close();
			f.Close();
		}

		/// <summary>
		/// Ulo�� obsah vektoru do souboru
		/// </summary>
		/// <param name="t">StreamWriter</param>
		public void Export(StreamWriter t) {
			t.WriteLine(this.Count);
			for(int i = 0; i < this.Count; i++)
				t.WriteLine(this[i]);
		}

		/// <summary>
		/// Na�te obsah vektoru ze souboru
		/// </summary>
		/// <param name="fName">Jm�no souboru</param>
		public void Import(string fName) {
			FileStream f = new FileStream(fName, FileMode.Open);
			StreamReader t = new StreamReader(f);

			this.Import(t);

			t.Close();
			f.Close();
		}
		
		/// <summary>
		/// Na�te obsah vektoru ze souboru
		/// </summary>
		/// <param name="t">StreamReader</param>
		public void Import(StreamReader t) {
			int count = int.Parse(t.ReadLine());

			for(int i = 0; i < count; i++)
				this.Add(t.ReadLine());
		}
	}
}
