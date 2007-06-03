using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor rovnosti
	/// </summary>
	public class Equals: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
        public override OperatorPriority Priority { get { return OperatorPriority.ComparePriority; } }

		protected override object EvaluateII(int left, int right) {
			return left == right;
		}

		protected override object EvaluateVVx(Vector left, Vector right) {
			return left == right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left == right;
		}

		protected override object EvaluateMMx(Matrix left, Matrix right) {
			return left == right;
		}

		protected override object EvaluateSSx(string left, string right) {
			return left == right;
		}

		private const string operatorName = "==";
	}
}
