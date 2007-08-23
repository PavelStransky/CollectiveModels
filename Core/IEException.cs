using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Výjimka pøi implementaci IExportable
    /// </summary>
    public class IEException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public IEException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public IEException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
