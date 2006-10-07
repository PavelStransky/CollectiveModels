using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Zapouzdøuje HotKey
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
        /// Vrátí hodnotu Key
        /// </summary>
        public char Key { get { return this.key; } }

        /// <summary>
        /// Spustí výpoèet
        /// </summary>
        public void Evaluate() {
            this.atom.Evaluate();
        }
    }
}
