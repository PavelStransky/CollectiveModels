using System;
using System.Collections;

namespace PavelStransky.Expression
{
    /// <summary>
    /// Možné události na kontextu
    /// </summary>
    public enum ContextEventType {
        GraphRequest,
        Change,
        ChangeDirectory,
        NewContext,
        SetContext,
        Save,
        Exit
    }

    /// <summary>
    /// Tøída k pøedávání informací o událostech na kontextu
    /// </summary>
    public class ContextEventArgs : EventArgs {
        // Typ události
        private ContextEventType eventType;
        // Parametry
        private ArrayList p = new ArrayList();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="eventType">Typ události</param>
        public ContextEventArgs(ContextEventType eventType) {
            this.eventType = eventType;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="eventType">Typ události</param>
        /// <param name="p">Parametry</param>
        public ContextEventArgs(ContextEventType eventType, params object[] p):this(eventType) {
            for(int i = 0; i < p.Length; i++)
                this.p.Add(p[i]);
        }

        /// <summary>
        /// Typ události
        /// </summary>
        public ContextEventType EventType { get { return this.eventType; } }

        /// <summary>
        /// Poèet parametrù
        /// </summary>
        public int NumParams { get { return this.p.Count; } }

        /// <summary>
        /// Vrátí parametr
        /// </summary>
        /// <param name="i">Index parametru</param>
        public object GetParam(int i) {
            if(i < this.p.Count)
                return this.p[i];
            else
                return null;
        }

        /// <summary>
        /// Vrátí parametr
        /// </summary>
        public object GetParam() {
            return this.GetParam(0);
        }
    }

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
    /// Tøída k pøedávání jména souboru
    /// </summary>
    public class FileNameEventArgs : EventArgs {
        // Jméno souboru
        private string fileName;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        public FileNameEventArgs(string fileName) {
            this.fileName = fileName;
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public FileNameEventArgs() { }

        /// <summary>
        /// Jméno souboru
        /// </summary>
        public string FileName { get { return this.fileName; } }
    }
}
