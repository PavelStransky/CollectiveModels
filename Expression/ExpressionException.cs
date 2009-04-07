using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Výjimka ve tøídì Expression
    /// </summary>
    public class ExpressionException: PositionTextException {
        private Expression expression = null;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz</param>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(Expression expression, string message, params int[] position)
            : base(expression.Expression, message, position) {
            this.expression = expression;
       }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz</param>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(Expression expression, string message, string detailMessage, params int[] position)
            : base(expression.Expression, message, detailMessage, position) {
            this.expression = expression;
        }

        /// <summary>
        /// Výraz
        /// </summary>
        public Expression Expression { get { return this.expression; } }
    }
}
