using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Výjimka ve tøídì Variable
    /// </summary>
    public class VariableException: PositionTextException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public VariableException(string text, string message, params int[] position)
            : base(text, message, position) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public VariableException(string text, string message, string detailMessage, params int[] position)
            : base(text, message, detailMessage) { }
    }
}
