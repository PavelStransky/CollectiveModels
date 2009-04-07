using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Výjimka ve tøídì Fnc
    /// </summary>
    public class FncException: DetailException {
        private Fnc fnc;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fnc">Funkce, ve které došlo k chybì</param>
        /// <param name="message">Text chybového hlášení</param>
        public FncException(Fnc fnc, string message) : base(message) {
            this.fnc = fnc;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fnc">Funkce, ve které došlo k chybì</param>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public FncException(Fnc fnc, string message, string detailMessage) : base(message, detailMessage) {
            this.fnc = fnc;
        }

        /// <summary>
        /// Funkce, ve které došlo k chybì
        /// </summary>
        public Fnc Fnc { get { return this.fnc; } }
    }
}
