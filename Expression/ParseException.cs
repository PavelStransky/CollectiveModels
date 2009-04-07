using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Výjimka pøi získávání hodnoty z øetìzce
    /// </summary>
    public class ParseException: Exception {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public ParseException(string message) : base(message) { }
    }
}
