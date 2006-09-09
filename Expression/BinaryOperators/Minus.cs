using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor minus
	/// </summary>
	public class Minus: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return addPriority;}}

		protected override object EvaluateII(int left, int right) {
			return left - right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left - right;
		}

		protected override object EvaluateDVx(double left, Vector right) {
			return left - right;
		}

		protected override object EvaluateDMx(double left, Matrix right) {
			return left - right;
		}

		protected override object EvaluatePPx(PointD left, PointD right) {
			return left - right;
		}

		protected override object EvaluateVDx(Vector left, double right) {
			return left - right;
		}

		protected override object EvaluateVVx(Vector left, Vector right) {
			return left - right;
		}

		protected override object EvaluateMDx(Matrix left, double right) {
			return left - right;
		}

		protected override object EvaluateMMx(Matrix left, Matrix right) {
			return left - right;
		}

		private const string operatorName = "-";
	}
}
