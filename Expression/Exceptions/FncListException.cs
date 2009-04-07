using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// V�jimka ve t��d� FncList
    /// </summary>
    public class FncListException: Exception {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public FncListException(string message) : base(message) { }
    }
}
