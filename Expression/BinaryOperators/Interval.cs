using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Interval 1...X
	/// </summary>
	public class Interval: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return multiplePriority;}}

		protected override object EvaluateII(int left, int right) {
			Array result = new Array();
			if(left < right)
				for(int i = left; i <= right; i++)
					result.Add(i);
			else
				for(int i = left; i >= right; i--)
					result.Add(i);
			return result;
		}

		private const string operatorName = "...";
	}
}
