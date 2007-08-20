using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Výraz
	/// </summary>
	public class Expression: Atom {
		// To, co poèítáme
		private object atom;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Expression(string expression)
            : base(expression, null) {
			this.parent = null;
            CheckSyntax(this.expression);

            this.expression = RemoveComments(this.expression);
            this.expression = FillBracket(this.expression);

            this.atom = CreateAtomObject(this.expression);
        }

		/// <summary>
		/// Provede výpoèet
		/// </summary>
		/// <returns>Výsledek výpoètu</returns>
        /// <param name="guider">Prùvodce výpoètu</param>
        public override object Evaluate(Guider guider) {
			object result = EvaluateAtomObject(guider, this.atom);
			return result;
		}
	}

	/// <summary>
	/// Výjimka ve tøídì Expression
	/// </summary>
	public class ExpressionException: DetailException {
        private int position;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ExpressionException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
		public ExpressionException(string message, int position) : base(errMessage + message) {
            this.position = position;
        }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		/// <param name="detailMessage">Detail chyby</param>
		public ExpressionException(string message, string detailMessage) : base(errMessage + message, detailMessage) {}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(string message, string detailMessage, int position) : base(errMessage + message, detailMessage) {
            this.position = position;
        }

        /// <summary>
        /// Pozice chyby
        /// </summary>
        public int Position { get { return this.position; } }

        private const string errMessage = "Pøi zpracování výrazu došlo k chybì: ";
	}
}
