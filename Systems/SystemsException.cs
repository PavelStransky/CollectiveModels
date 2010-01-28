using System;

using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// Výjimka v sestavení Systems
    /// </summary>
    public class SystemsException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public SystemsException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public SystemsException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
