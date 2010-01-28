using System;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// Výjimka ve tøídì GCM
    /// </summary>
    public class GCMException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public GCMException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public GCMException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
