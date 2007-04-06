using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Promìnná
	/// </summary>
	public class Variable {
		// Jméno objektu
		private string name;
		// Poèet instancí objektu
		private static int count = 0;
		// Hodnota promìnné
		private object item;

		/// <summary>
		/// Jméno objektu
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Hodnota promìnné
		/// </summary>
        public object Item { get { return this.item; } set { this.item = value; } }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="name">Jméno objektu</param>
		/// <param name="expression">Výraz</param>
		public Variable(string name, object item) {
			if(name == string.Empty)
				this.name = string.Format("{0}{1}", defaultName, count++);
			else
				this.name = name;

			this.item = item;
		}

		private string defaultName = "Object";
	}
}
