using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Zapouzd�uje HotKey
	/// </summary>
    public class HotKey {
        private Atom atom;
        private char key;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public HotKey(Atom atom, char key) {
            this.atom = atom;
            this.key = key.ToString().ToUpper()[0];
        }

        /// <summary>
        /// Vr�t� hodnotu Key
        /// </summary>
        public char Key { get { return this.key; } }

        /// <summary>
        /// Spust� v�po�et
        /// </summary>
        public void Evaluate() {
            this.atom.Evaluate();
        }
    }
}
