using System;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.IBM {
    /// <summary>
    /// Výjimka ve tøídì IBM
    /// </summary>
    public class IBMException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public IBMException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public IBMException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public IBMException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
