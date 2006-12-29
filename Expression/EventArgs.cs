using System;

namespace PavelStransky.Expression
{
	/// <summary>
	/// T��da k p�ed�v�n� ��dosti o vytvo�en� nov�ho grafu
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
	/// T��da k p�ed�v�n� informac� p�i zm�n� prom�nn�
	/// </summary>
	public class ChangeEventArgs: EventArgs {
		private Context context;
		// P�vodn� objekt
		private object oldItem;
		// Nov� objekt
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
    /// T��da k p�ed�v�n� kontextu
    /// </summary>
    public class ContextEventArgs : EventArgs {
        // Nov� kontext
        private Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Nov� kontext</param>
        public ContextEventArgs(Context context) {
            this.context = context;
        }

        /// <summary>
        /// Nov� kontext
        /// </summary>
        public Context Context { get { return this.context; } }
    }
}
