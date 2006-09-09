using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.UnaryOperators {
	/// <summary>
	/// Operátor + (nic nedìlá)
	/// </summary>
	public class Plus: UnaryOperator {
		public override string OperatorName {get {return operatorName;}}

		protected override object EvaluateI(int item) {
			return item;
		}

		protected override object EvaluateD(double item) {
			return item;
		}

		protected override object EvaluateP(PointD item) {
			return item;
		}

		protected override object EvaluateV(Vector item) {
			return item;
		}

		protected override object EvaluatePv(PointVector item) {
			return item;
		}

		protected override object EvaluateM(Matrix item) {
			return item;
		}

		protected override object EvaluateS(string item) {
			return item;
		}

		private const string operatorName = "+";
	}
}
