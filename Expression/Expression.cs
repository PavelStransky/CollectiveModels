using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Výraz
	/// </summary>
	public class Expression: Atom {
		// To, co poèítáme
		private object atom;

		// True, pokud nechceme vracet výsledek (rozlišuje se podle støedníku na konci výrazu)
		private bool noResult;

		/// <summary>
		/// Vrací true, pokud nezobrazujeme výsledek
		/// </summary>
		public bool NoResult {get {return this.noResult;}}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz funkce</param>
        public Expression(string expression)
            : this(expression, null) {
        }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Expression(string expression, IOutputWriter writer)
            : base(expression, null, writer) {
			if(this.expression[this.expression.Length - 1] != semicolon)
				throw new ExpressionException(errorMessageNoEndSemicolon, string.Format(errorMessageNoEndSemicolonDetail, this.expression));

			if(this.expression.Length > 2 && this.expression[this.expression.Length - 2] == semicolon)
				this.noResult = true;
			else
				this.noResult = false;

			this.expression = RemoveSemicolons(this.expression);
			this.parent = null;
			this.atom = this.CreateAtomObject(this.expression);
		}

		/// <summary>
		/// Provede výpoèet
		/// </summary>
		/// <returns>Výsledek výpoètu</returns>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        public override object Evaluate(Context context) {
			object result = EvaluateAtomObject(context, this.atom);

			if(this.noResult)
				result = null;

			return result;
		}

		/// <summary>
		/// Odstraní všechny støedníky z konce výrazu
		/// </summary>
		/// <param name="e">Výraz</param>
		private static string RemoveSemicolons(string e) {
			for(int i = e.Length - 1; i >= 0; i--)
				if(e[i] != separator)
					return e.Substring(0, i + 1);

			return string.Empty;
		}

		private const string errorMessageNoEndSemicolon = "Výraz není ukonèen znakem ';'";
		private const string errorMessageNoEndSemicolonDetail = "Výraz: {0}";
	}

	/// <summary>
	/// Výjimka ve tøídì Expression
	/// </summary>
	public class ExpressionException: DetailException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ExpressionException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ExpressionException(string message, Exception innerException) : base(errMessage + message, innerException) {}

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
        /// <param name="detailMessage">Detail chyby</param>
        public ExpressionException(string message, string detailMessage, Exception innerException) : base(errMessage + message, detailMessage, innerException) { }

        private const string errMessage = "Pøi zpracování výrazu došlo k chybì: ";
	}
}
