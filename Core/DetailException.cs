using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// V�jimka s detailem
    /// </summary>
    public abstract class DetailException: ApplicationException {
        private string detailMessage = string.Empty;

        /// <summary>
        /// P��davn� informace o v�jimce
        /// </summary>
        public string DetailMessage { get { return this.detailMessage; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public DetailException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public DetailException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public DetailException(string message, string detailMessage)
            : this(message) {
            this.detailMessage = detailMessage;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public DetailException(string message, string detailMessage, Exception innerException)
            : this(message, innerException) {
            this.detailMessage = detailMessage;
        }
    }
}
