using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms
{
	/// <summary>
	/// Historie pøíkazù
	/// </summary>
	public class History: ArrayList
	{
		// Index aktuálního záznamu
		private int index = 0;
		// Aktuální kontext
		private Context context;

		/// <summary>
		/// Index aktuálního záznamu
		/// </summary>
		public int Index {get {return this.index;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="context">Aktuální kontext ke zpracování</param>
		public History(Context context): base() {
			this.context = context;
		}

		/// <summary>
		/// Pøidá øetìzec s obsahem pøíkazu do øady
		/// </summary>
		/// <param name="value">Øetìzec pøíkazu</param>
		/// <returns>Index nového záznamu</returns>
		public override int Add(object value) {
			this.index = 0;
			return base.Add (value);
		}

		/// <summary>
		/// Vynulování historie (musíme vynulovat i index)
		/// </summary>
		public override void Clear() {
			this.index = -1;
			base.Clear ();
		}

		/// <summary>
		/// Vrátí øádky všechny øádky, každou ve vlastním stringu
		/// </summary>
		public string [] GetAllLines() {
			string [] lines = new string[this.Count];
			for(int i = 0; i < this.Count; i++)
				lines[i] = this[i] as string;
			return lines;
		}

		/// <summary>
		/// Nastaví všechny øádky podle vstupu
		/// </summary>
		public void SetAllLines(string [] lines) {
			this.Clear();

			for(int i = 0; i < lines.Length; i++)
				this.Add(lines[i]);
		}

		/// <summary>
		/// Pøeète øádku na aktuálním indexu a aktuální index posune zpìt
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
		/// Pøeète øádku na aktuálním indexu a aktuální index posune vpøed
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
		/// Spustí všechny pøíkazy v historii
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
		/// Uloží obsah historie do souboru
		/// </summary>
		/// <param name="fName">Jméno souboru</param>
		public void Export(string fName) {
			FileStream f = new FileStream(fName, FileMode.Create);
			StreamWriter t = new StreamWriter(f);

			this.Export(t);

			t.Close();
			f.Close();
		}

		/// <summary>
		/// Uloží obsah vektoru do souboru
		/// </summary>
		/// <param name="t">StreamWriter</param>
		public void Export(StreamWriter t) {
			t.WriteLine(this.Count);
			for(int i = 0; i < this.Count; i++)
				t.WriteLine(this[i]);
		}

		/// <summary>
		/// Naète obsah vektoru ze souboru
		/// </summary>
		/// <param name="fName">Jméno souboru</param>
		public void Import(string fName) {
			FileStream f = new FileStream(fName, FileMode.Open);
			StreamReader t = new StreamReader(f);

			this.Import(t);

			t.Close();
			f.Close();
		}
		
		/// <summary>
		/// Naète obsah vektoru ze souboru
		/// </summary>
		/// <param name="t">StreamReader</param>
		public void Import(StreamReader t) {
			int count = int.Parse(t.ReadLine());

			for(int i = 0; i < count; i++)
				this.Add(t.ReadLine());
		}
	}
}
