using System;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Výjimka ve tøídì GCM
    /// </summary>
    public class GCMException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public GCMException(string message) : base(errMessage + message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public GCMException(string message, Exception innerException) : base(errMessage + message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public GCMException(string message, string detailMessage) : base(errMessage + message, detailMessage) { }

        private const string errMessage = "Ve tøídì GCM došlo k chybì: ";
    }
}
