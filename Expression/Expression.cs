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
            this.expression = RemoveEndSeparator(this.expression);
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
        private int position1 = -1;
        private int position2 = -1;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(string message, params int[] position)
            : base(errMessage + message) {
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
        public ExpressionException(string message, string detailMessage, params int[] position) : base(errMessage + message, detailMessage) {
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

        private const string errMessage = "Pøi zpracování výrazu došlo k chybì: ";
	}
}
