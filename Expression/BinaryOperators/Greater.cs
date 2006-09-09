using System;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor vìtší
	/// </summary>
	public class Greater: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return comparePriority;}}

		protected override object EvaluateII(int left, int right) {
			return left > right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left > right;
		}

		private const string operatorName = ">";
	}
}
