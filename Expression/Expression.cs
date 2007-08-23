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
            Highlight highlight = CheckSyntax(this.expression);
            highlight.ThrowErrors();

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
}
