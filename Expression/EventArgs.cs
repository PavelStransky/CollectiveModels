using System;

namespace PavelStransky.Expression
{
	/// <summary>
	/// Tøída k pøedávání žádosti o vytvoøení nového grafu
	/// </summary>
	public class GraphRequestEventArgs: EventArgs {
		private Variable variable;
		private Context context;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public GraphRequestEventArgs(Context context, Variable variable) {
			this.context = context;
			this.variable = variable;
		}

		public Context Context {get {return this.context;}}
		public Variable Variable {get {return this.variable;}}
	}

	/// <summary>
	/// Tøída k pøedávání informací pøi zmìnì promìnné
	/// </summary>
	public class ChangeEventArgs: EventArgs {
		private Context context;
		// Pùvodní objekt
		private object oldItem;
		// Nový objekt
		private object newItem;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public ChangeEventArgs(Context context, object oldItem, object newItem) {
			this.context = context;
			this.oldItem = oldItem;
			this.newItem = newItem;
		}

		public Context Context {get {return this.context;}}
		public object OldItem {get {return this.oldItem;}}
		public object NewItem {get {return this.newItem;}}
	}
}
