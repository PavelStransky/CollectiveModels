using System;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Oper�tor men�� a rovno
	/// </summary>
	public class LEquals: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
        public override OperatorPriority Priority { get { return OperatorPriority.ComparePriority; } }

		protected override object EvaluateII(int left, int right) {
			return left <= right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left <= right;
		}

		private const string operatorName = "<=";
	}
}