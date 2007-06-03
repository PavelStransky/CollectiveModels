using System;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor vìtší nebo rovno
	/// </summary>
	public class GEquals: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override OperatorPriority Priority {get {return OperatorPriority.ComparePriority;}}

		protected override object EvaluateII(int left, int right) {
			return left >= right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left >= right;
		}

		private const string operatorName = ">=";
	}
}
