using System;

using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// V�jimka v sestaven� Systems
    /// </summary>
    public class SystemsException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public SystemsException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public SystemsException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
