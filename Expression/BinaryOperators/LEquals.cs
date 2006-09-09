using System;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor menší a rovno
	/// </summary>
	public class LEquals: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return comparePriority;}}

		protected override object EvaluateII(int left, int right) {
			return left <= right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left <= right;
		}

		private const string operatorName = "<=";
	}
}
