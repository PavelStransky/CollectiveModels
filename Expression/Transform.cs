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
        /// <param name="writer">Writer pro textov� v�stupy</param>
        public Transform(string expression, Atom parent, IOutputWriter writer)
            : base(expression, parent, writer) {
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
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
		/// <returns>V�sledek v�po�tu</returns>
		public override object Evaluate(Context context) {
			object operandResult = EvaluateAtomObject(context, this.operand);
			return this.unaryOperator.Evaluate(operandResult);
		}

		private const string errorMessageNoOperator = "V ��sti v�razu {0} nelze nal�zt oper�tor ke zpracov�n�.";
	}
}
