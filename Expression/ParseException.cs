using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// V�jimka p�i z�sk�v�n� hodnoty z �et�zce
    /// </summary>
    public class ParseException: Exception {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public ParseException(string message) : base(message) { }
    }
}
