using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// V�jimka ve t��d� Atom
    /// </summary>
    public class AtomException: PositionTextException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="position">Pozice chyby</param>
        public AtomException(string text, string message, params int[] position)
            : base(text, message, position) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chybov�ho hl�en�</param>
        /// <param name="position">Pozice chyby</param>
        public AtomException(string text, string message, string detailMessage, params int[] position)
            : base(text, message, detailMessage) { }
    }
}
