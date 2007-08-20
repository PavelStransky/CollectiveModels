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
            this.expression = RemoveEndSeparator(this.expression);
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
        private int position1 = -1;
        private int position2 = -1;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
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
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="position">Pozice chyby</param>
        public ExpressionException(string message, string detailMessage, params int[] position) : base(errMessage + message, detailMessage) {
            if(position.Length > 0)
                this.position1 = position[0];
            if(position.Length > 1)
                this.position2 = position[1];
        }

        /// <summary>
        /// Pozice chyby (za��tek)
        /// </summary>
        public int Position1 { get { return this.position1; } }

        /// <summary>
        /// Pozice chyby (konec)
        /// </summary>
        public int Position2 { get { return this.position2; } }

        private const string errMessage = "P�i zpracov�n� v�razu do�lo k chyb�: ";
	}
}
