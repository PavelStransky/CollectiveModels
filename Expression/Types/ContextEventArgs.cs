using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
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
        SaveNow,
        Exit
    }

    /// <summary>
    /// T��da k p�ed�v�n� informac� o ud�lostech na kontextu
    /// </summary>
    public class ContextEventArgs: EventArgs {
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
        public ContextEventArgs(ContextEventType eventType, params object[] p)
            : this(eventType) {
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
}
