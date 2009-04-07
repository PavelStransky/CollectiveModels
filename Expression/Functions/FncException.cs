using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// V�jimka ve t��d� Fnc
    /// </summary>
    public class FncException: DetailException {
        private Fnc fnc;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fnc">Funkce, ve kter� do�lo k chyb�</param>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public FncException(Fnc fnc, string message) : base(message) {
            this.fnc = fnc;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fnc">Funkce, ve kter� do�lo k chyb�</param>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public FncException(Fnc fnc, string message, string detailMessage) : base(message, detailMessage) {
            this.fnc = fnc;
        }

        /// <summary>
        /// Funkce, ve kter� do�lo k chyb�
        /// </summary>
        public Fnc Fnc { get { return this.fnc; } }
    }
}
