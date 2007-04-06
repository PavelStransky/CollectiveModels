using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Prom�nn�
	/// </summary>
	public class Variable {
		// Jm�no objektu
		private string name;
		// Po�et instanc� objektu
		private static int count = 0;
		// Hodnota prom�nn�
		private object item;

		/// <summary>
		/// Jm�no objektu
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Hodnota prom�nn�
		/// </summary>
        public object Item { get { return this.item; } set { this.item = value; } }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="name">Jm�no objektu</param>
		/// <param name="expression">V�raz</param>
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
