using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor dìleno
	/// </summary>
	public class Divide: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return multiplePriority;}}

		protected override object EvaluateII(int left, int right) {
			return left / right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left / right;
		}

		protected override object EvaluateDMx(double left, Matrix right) {
			return left / right;
		}

		protected override object EvaluatePPx(PointD left, PointD right) {
			return left / right;
		}

		protected override object EvaluateVDx(Vector left, double right) {
			return left / right;
		}

		protected override object EvaluateVMx(Vector left, Matrix right) {
			return left * right.Inv();
		}

		protected override object EvaluatePvPx(PointVector left, PointD right) {
			return left / right;
		}

		protected override object EvaluateMDx(Matrix left, double right) {
			return left / right;
		}

		protected override object EvaluateMMx(Matrix left, Matrix right) {
			return left / right;
		}

		private const string operatorName = "/";
	}
}
