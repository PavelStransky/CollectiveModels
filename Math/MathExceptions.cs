using System;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Výjimka v sestavení Math
    /// </summary>
    public class MathException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public MathException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public MathException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
