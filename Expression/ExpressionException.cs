using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Výjimka ve tøídì Expression
    /// </summary>
    public class ExpressionException: DetailException {
        private int position1 = -1;
        private int position2 = -1;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(string message, params int[] position)
            : base(message) {
            if(position.Length > 0)
                this.position1 = position[0];
            if(position.Length > 1)
                this.position2 = position[1];
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(string message, string detailMessage, params int[] position)
            : base(message, detailMessage) {
            if(position.Length > 0)
                this.position1 = position[0];
            if(position.Length > 1)
                this.position2 = position[1];
        }

        /// <summary>
        /// Pozice chyby (zaèátek)
        /// </summary>
        public int Position1 { get { return this.position1; } }

        /// <summary>
        /// Pozice chyby (konec)
        /// </summary>
        public int Position2 { get { return this.position2; } }
    }
}
