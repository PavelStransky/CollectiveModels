using System;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// V�jimka v sestaven� Math
    /// </summary>
    public class MathException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public MathException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public MathException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
