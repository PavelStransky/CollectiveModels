using System;
using System.Collections;

namespace PavelStransky.Expression
{
    /// <summary>
    /// Mo�n� ud�losti na kontextu
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
    /// T��da k p�ed�v�n� informac� o ud�lostech na kontextu
    /// </summary>
    public class ContextEventArgs : EventArgs {
        // Typ ud�losti
        private ContextEventType eventType;
        // Parametry
        private ArrayList p = new ArrayList();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="eventType">Typ ud�losti</param>
        public ContextEventArgs(ContextEventType eventType) {
            this.eventType = eventType;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="eventType">Typ ud�losti</param>
        /// <param name="p">Parametry</param>
        public ContextEventArgs(ContextEventType eventType, params object[] p):this(eventType) {
            for(int i = 0; i < p.Length; i++)
                this.p.Add(p[i]);
        }

        /// <summary>
        /// Typ ud�losti
        /// </summary>
        public ContextEventType EventType { get { return this.eventType; } }

        /// <summary>
        /// Po�et parametr�
        /// </summary>
        public int NumParams { get { return this.p.Count; } }

        /// <summary>
        /// Vr�t� parametr
        /// </summary>
        /// <param name="i">Index parametru</param>
        public object GetParam(int i) {
            if(i < this.p.Count)
                return this.p[i];
            else
                return null;
        }

        /// <summary>
        /// Vr�t� parametr
        /// </summary>
        public object GetParam() {
            return this.GetParam(0);
        }
    }

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
    /// T��da k p�ed�v�n� jm�na souboru
    /// </summary>
    public class FileNameEventArgs : EventArgs {
        // Jm�no souboru
        private string fileName;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        public FileNameEventArgs(string fileName) {
            this.fileName = fileName;
        }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public FileNameEventArgs() { }

        /// <summary>
        /// Jm�no souboru
        /// </summary>
        public string FileName { get { return this.fileName; } }
    }
}
