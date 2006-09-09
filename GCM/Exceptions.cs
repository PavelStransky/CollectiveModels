using System;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// V�jimka ve t��d� GCM
    /// </summary>
    public class GCMException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public GCMException(string message) : base(errMessage + message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public GCMException(string message, Exception innerException) : base(errMessage + message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public GCMException(string message, string detailMessage) : base(errMessage + message, detailMessage) { }

        private const string errMessage = "Ve t��d� GCM do�lo k chyb�: ";
    }
}
