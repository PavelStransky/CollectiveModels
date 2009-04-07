using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Výjimka ve tøídì FncList
    /// </summary>
    public class FncListException: Exception {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public FncListException(string message) : base(message) { }
    }
}
