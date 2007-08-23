using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
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
}
