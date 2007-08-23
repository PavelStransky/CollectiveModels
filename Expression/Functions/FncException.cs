using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// V�jimka ve t��d� Fnc
    /// </summary>
    public class FncException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public FncException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public FncException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public FncException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
