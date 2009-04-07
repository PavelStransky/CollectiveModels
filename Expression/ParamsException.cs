using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// V�jimka mezi parametry
    /// </summary>
    public class ParamsException: DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public ParamsException(string message) : base(message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public ParamsException(string message, string detailMessage) : base(message, detailMessage) { }
    }
}
