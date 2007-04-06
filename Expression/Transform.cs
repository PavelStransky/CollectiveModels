using System;
using System.Text.RegularExpressions;

using PavelStransky.Math;
using PavelStransky.Expression.UnaryOperators;

namespace PavelStransky.Expression {
	/// <summary>
	/// Unární operátor
	/// </summary>
	public class Transform: Atom {
		// Typ operátoru
		private UnaryOperator unaryOperator;
		// Operand
		private object operand;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz</param>
        /// <param name="parent">Rodiè</param>
        public Transform(string expression, Atom parent)
            : base(expression, parent) {
			string operatorName = string.Empty;
			int pos = FindUnaryOperatorPosition(out operatorName, this.expression);

			if(pos != 0)
				throw new ExpressionException(string.Format(errorMessageNoOperator, this.expression));

			this.unaryOperator = unaryOperators[operatorName] as UnaryOperator;
			
			string operandString = RemoveOutsideBracket(this.expression.Substring(operatorName.Length, this.expression.Length - operatorName.Length)).Trim();
			this.operand = this.CreateAtomObject(operandString);
		}

		/// <summary>
		/// Provede výpoèet vzorce
		/// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
		/// <returns>Výsledek výpoètu</returns>
        public override object Evaluate(Guider guider) {
            object operandResult = EvaluateAtomObject(guider, this.operand);
			return this.unaryOperator.Evaluate(operandResult);
		}

		private const string errorMessageNoOperator = "V èásti výrazu {0} nelze nalézt operátor ke zpracování.";
	}
}
