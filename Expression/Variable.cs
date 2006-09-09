using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Prom�nn�
	/// </summary>
	public class Variable {
		// Ud�lost vyvolan� po zm�n� prom�nn�
		public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);
		public event ChangeEventHandler Change;

		/// <summary>
		/// Vol� se p�i zm�n� prom�nn�
		/// </summary>
		public void OnChange(ChangeEventArgs e) {
			if(this.Change != null)
				this.Change(this, e);
		}

		// Kontext, do kter�ho bude objekt ulo�en
		protected Context context;
		// Jm�no objektu
		private string name;
		// Po�et instanc� objektu
		private static int count = 0;
		// V�raz k v�po�tu
		private Assignment assignment;
		// Hodnota prom�nn�
		private object item;

		/// <summary>
		/// Jm�no objektu
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Vzorec ve stringov� hodnot�
		/// </summary>
		public string Expression {get {return this.assignment == null ? string.Empty : this.assignment.Expression;}}

		/// <summary>
		/// Nastav� vzorec k v�po�tu
		/// </summary>
		public Assignment Assignment {set {this.assignment = value;}}

		/// <summary>
		/// Obsahuje prom�nn� vzorec pro p�i�azen�?
		/// </summary>
		public bool IsAssignment {get {return this.assignment != null;}}

		/// <summary>
		/// Hodnota prom�nn�
		/// </summary>
		public object Item {
			get {
				return this.item;
			}
			set {
				object oldItem = this.item;
				this.item = value;
				this.OnChange(new ChangeEventArgs(this.context, oldItem, this.item));
			}
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="context">Kontext, do kter�ho bude objekt ulo�en</param>
		/// <param name="name">Jm�no objektu</param>
		/// <param name="expression">V�raz</param>
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
		/// Provede v�po�et v�razu (pokud n�jak� v�raz je)
		/// </summary>
		public object Evaluate() {
			if(this.assignment == null)
				throw new ExpressionException(string.Format(errorMessageNoExpression, this.name));
			return this.assignment.Evaluate();
		}

		private string defaultName = "Object";
		private string errorMessageNoExpression = "Prom�nnou {0} nelze vypo��tat, proto�e u n� nen� zad�n v�raz.";
	}
}
