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
            Highlight highlight = CheckSyntax(this.expression);
            highlight.ThrowErrors();

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
}
