using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// V�jimka ve t��d� FileData
    /// </summary>
    public class FileDataException: Exception {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public FileDataException(string message) : base(message) { }
    }
}
