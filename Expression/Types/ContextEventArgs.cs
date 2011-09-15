using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
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
        SaveNow,
        Exit
    }

    /// <summary>
    /// Tøída k pøedávání informací o událostech na kontextu
    /// </summary>
    public class ContextEventArgs: EventArgs {
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
        public ContextEventArgs(ContextEventType eventType, params object[] p)
            : this(eventType) {
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
}
