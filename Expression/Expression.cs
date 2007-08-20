using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// V�raz
	/// </summary>
	public class Expression: Atom {
		// To, co po��t�me
		private object atom;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz funkce</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
        public Expression(string expression)
            : base(expression, null) {
			this.parent = null;
            CheckSyntax(this.expression);

            this.expression = RemoveComments(this.expression);
            this.expression = FillBracket(this.expression);

            this.atom = CreateAtomObject(this.expression);
        }

		/// <summary>
		/// Provede v�po�et
		/// </summary>
		/// <returns>V�sledek v�po�tu</returns>
        /// <param name="guider">Pr�vodce v�po�tu</param>
        public override object Evaluate(Guider guider) {
			object result = EvaluateAtomObject(guider, this.atom);
			return result;
		}
	}

	/// <summary>
	/// V�jimka ve t��d� Expression
	/// </summary>
	public class ExpressionException: DetailException {
        private int position;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ExpressionException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="position">Pozice chyby</param>
		public ExpressionException(string message, int position) : base(errMessage + message) {
            this.position = position;
        }

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
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(string message, string detailMessage, int position) : base(errMessage + message, detailMessage) {
            this.position = position;
        }

        /// <summary>
        /// Pozice chyby
        /// </summary>
        public int Position { get { return this.position; } }

        private const string errMessage = "P�i zpracov�n� v�razu do�lo k chyb�: ";
	}
}
