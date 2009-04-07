using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// V�jimka ve t��d� Expression
    /// </summary>
    public class ExpressionException: PositionTextException {
        private Expression expression = null;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz</param>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(Expression expression, string message, params int[] position)
            : base(expression.Expression, message, position) {
            this.expression = expression;
       }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz</param>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(Expression expression, string message, string detailMessage, params int[] position)
            : base(expression.Expression, message, detailMessage, position) {
            this.expression = expression;
        }

        /// <summary>
        /// V�raz
        /// </summary>
        public Expression Expression { get { return this.expression; } }
    }
}
