using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// V�raz
	/// </summary>
	public class Expression: Atom {
		// To, co po��t�me
		private object atom;

		// True, pokud nechceme vracet v�sledek (rozli�uje se podle st�edn�ku na konci v�razu)
		private bool noResult;

		/// <summary>
		/// Vrac� true, pokud nezobrazujeme v�sledek
		/// </summary>
		public bool NoResult {get {return this.noResult;}}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz funkce</param>
        public Expression(string expression)
            : this(expression, null) {
        }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz funkce</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
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
		/// Provede v�po�et
		/// </summary>
		/// <returns>V�sledek v�po�tu</returns>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        public override object Evaluate(Context context) {
			object result = EvaluateAtomObject(context, this.atom);

			if(this.noResult)
				result = null;

			return result;
		}

		/// <summary>
		/// Odstran� v�echny st�edn�ky z konce v�razu
		/// </summary>
		/// <param name="e">V�raz</param>
		private static string RemoveSemicolons(string e) {
			for(int i = e.Length - 1; i >= 0; i--)
				if(e[i] != separator)
					return e.Substring(0, i + 1);

			return string.Empty;
		}

		private const string errorMessageNoEndSemicolon = "V�raz nen� ukon�en znakem ';'";
		private const string errorMessageNoEndSemicolonDetail = "V�raz: {0}";
	}

	/// <summary>
	/// V�jimka ve t��d� Expression
	/// </summary>
	public class ExpressionException: DetailException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ExpressionException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ExpressionException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		/// <param name="detailMessage">Detail chyby</param>
		public ExpressionException(string message, string detailMessage) : base(errMessage + message, detailMessage) {}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public ExpressionException(string message, string detailMessage, Exception innerException) : base(errMessage + message, detailMessage, innerException) { }

        private const string errMessage = "P�i zpracov�n� v�razu do�lo k chyb�: ";
	}
}
