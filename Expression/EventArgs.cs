using System;

namespace PavelStransky.Expression
{
	/// <summary>
	/// Tøída k pøedávání žádosti o vytvoøení nového grafu
	/// </summary>
	public class GraphRequestEventArgs: EventArgs {
		private TArray graphs;
        private string name;
        private int numColumns;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public GraphRequestEventArgs(TArray graphs, string name, int numColumns) {
			this.graphs = graphs;
            this.name = name;
            this.numColumns = numColumns;
		}

        public TArray Graphs { get { return this.graphs; } }
        public string Name { get { return this.name; } }
        public int NumColumns { get { return this.numColumns; } }
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

    /// <summary>
    /// Tøída k pøedávání kontextu
    /// </summary>
    public class ContextEventArgs : EventArgs {
        // Nový kontext
        private Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Nový kontext</param>
        public ContextEventArgs(Context context) {
            this.context = context;
        }

        /// <summary>
        /// Nový kontext
        /// </summary>
        public Context Context { get { return this.context; } }
    }
}
