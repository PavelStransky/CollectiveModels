using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Promìnná
	/// </summary>
	public class Variable {
		// Kontext, do kterého bude objekt uložen
		protected Context context;
		// Jméno objektu
		private string name;
		// Poèet instancí objektu
		private static int count = 0;
		// Výraz k výpoètu
		private Assignment assignment;
		// Hodnota promìnné
		private object item;

		/// <summary>
		/// Jméno objektu
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Vzorec ve stringové hodnotì
		/// </summary>
		public string Expression {get {return this.assignment == null ? string.Empty : this.assignment.Expression;}}

		/// <summary>
		/// Nastaví vzorec k výpoètu
		/// </summary>
		public Assignment Assignment {set {this.assignment = value;}}

		/// <summary>
		/// Obsahuje promìnná vzorec pro pøiøazení?
		/// </summary>
		public bool IsAssignment {get {return this.assignment != null;}}

		/// <summary>
		/// Hodnota promìnné
		/// </summary>
        public object Item { get { return this.item; } set { this.item = value; } }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="context">Kontext, do kterého bude objekt uložen</param>
		/// <param name="name">Jméno objektu</param>
		/// <param name="expression">Výraz</param>
		public Variable(Context context, string name, object item, Assignment assignment) {
			if(name == string.Empty)
				this.name = string.Format("{0}{1}", defaultName, count++);
			else
				this.name = name;

			this.item = item;
			this.context = context;
			this.assignment = assignment;
		}

		/// <summary>
		/// Provede výpoèet výrazu (pokud nìjaký výraz je)
		/// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        public object Evaluate(Context context) {
			if(this.assignment == null)
				throw new ExpressionException(string.Format(errorMessageNoExpression, this.name));
			return this.assignment.Evaluate(context);
		}

		private string defaultName = "Object";
		private string errorMessageNoExpression = "Promìnnou {0} nelze vypoèítat, protože u ní není zadán výraz.";
	}
}
