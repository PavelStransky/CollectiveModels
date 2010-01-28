using System;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// V�jimka ve t��d� GCM
    /// </summary>
    public class GCMException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public GCMException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public GCMException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
