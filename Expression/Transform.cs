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
		/// <param name="context">Kontext</param>
		/// <param name="expression">Výraz</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Transform(Context context, string expression, Atom parent, IOutputWriter writer)
            : base(context, expression, parent, writer) {
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
		/// <returns>Výsledek výpoètu</returns>
		public override object Evaluate() {
			object result = base.Evaluate();

			object operandResult = EvaluateAtomObject(this.context, this.operand);
			return this.unaryOperator.Evaluate(operandResult);
		}

		private const string errorMessageNoOperator = "V èásti výrazu {0} nelze nalézt operátor ke zpracování.";
	}
}
