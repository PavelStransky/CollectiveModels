using System;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression.UnaryOperators {
	/// <summary>
	/// Operátor - (obrací znaménko, resp. poøadí znakù ve stringu)
	/// </summary>
	public class Minus: UnaryOperator {
		public override string OperatorName {get {return operatorName;}}

		protected override object EvaluateI(int item) {
			return -item;
		}

		protected override object EvaluateD(double item) {
			return -item;
		}

		protected override object EvaluateP(PointD item) {
			return new PointD(-item.X, -item.Y);
		}

		protected override object EvaluateV(Vector item) {
			return item * (-1.0);
		}

		protected override object EvaluatePv(PointVector item) {
			return item * new PointD(-1.0, -1.0);
		}

		protected override object EvaluateM(Matrix item) {
			return item * (-1.0);
		}

		protected override object EvaluateS(string item) {
			StringBuilder s = new StringBuilder(item.Length);
			for(int i = item.Length - 1; i >= 0; i--)
				s.Append(item[i]);
			return s.ToString();
		}

		private const string operatorName = "-";
	}
}
