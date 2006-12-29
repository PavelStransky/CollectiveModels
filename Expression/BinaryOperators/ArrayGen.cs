using System;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Identické kopírování prvku do øady
	/// (prvek # poèet)
	/// </summary>
	public class ArrayGen: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return powerPriority;}}

		protected override object EvaluateII(int left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluateDI(double left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluatePI(PointD left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluatePvI(PointVector left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluateVI(Vector left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluateMI(Matrix left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluateSI(string left, int right) {
			return this.EvaluateArray(left, right);
		}

		protected override object EvaluateA(TArray left, object right) {
			if(right is int) 
				return this.EvaluateArray(left, (int)right);
			else
				return this.UnknownType(left, right);
		}

		private object EvaluateArray(object left, int right) {
			TArray result = new TArray();
			for(int i = 0; i < right; i++)
				result.Add(left);
			return result;
		}

		private const string operatorName = "#";
	}
}
