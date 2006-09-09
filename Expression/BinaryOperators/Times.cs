using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor krát
	/// </summary>
	public class Times: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return multiplePriority;}}

		protected override object EvaluateII(int left, int right) {
			return left * right;
		}

		protected override object EvaluateDDx(double left, double right) {
			return left * right;
		}

		protected override object EvaluateDVx(double left, Vector right) {
			return right * left;
		}

		protected override object EvaluateDMx(double left, Matrix right) {
			return right * left;
		}

		protected override object EvaluatePPx(PointD left, PointD right) {
			return left * right;
		}
		
		protected override object EvaluatePPvx(PointD left, PointVector right) {
			return right * left;
		}

		protected override object EvaluateVDx(Vector left, double right) {
			return left * right;
		}

		protected override object EvaluateVVx(Vector left, Vector right) {
			return left * right;
		}

		protected override object EvaluateVMx(Vector left, Matrix right) {
			return left * right;
		}

		protected override object EvaluatePvPx(PointVector left, PointD right) {
			return left * right;
		}

		protected override object EvaluateMDx(Matrix left, double right) {
			return left * right;
		}

		protected override object EvaluateMVx(Matrix left, Vector right) {
			return left * right;
		}

		protected override object EvaluateMMx(Matrix left, Matrix right) {
			return left * right;
		}

		private const string operatorName = "*";
	}
}
