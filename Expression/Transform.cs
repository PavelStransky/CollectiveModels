using System;
using System.Text.RegularExpressions;

using PavelStransky.Math;
using PavelStransky.Expression.UnaryOperators;

namespace PavelStransky.Expression {
	/// <summary>
	/// Un�rn� oper�tor
	/// </summary>
	public class Transform: Atom {
		// Typ oper�toru
		private UnaryOperator unaryOperator;
		// Operand
		private object operand;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz</param>
        /// <param name="parent">Rodi�</param>
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
		/// Provede v�po�et vzorce
		/// </summary>
        /// <param name="guider">Pr�vodce v�po�tu</param>
		/// <returns>V�sledek v�po�tu</returns>
        public override object Evaluate(Guider guider) {
            object operandResult = EvaluateAtomObject(guider, this.operand);
			return this.unaryOperator.Evaluate(operandResult);
		}

		private const string errorMessageNoOperator = "V ��sti v�razu {0} nelze nal�zt oper�tor ke zpracov�n�.";
	}
}
