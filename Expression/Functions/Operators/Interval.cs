using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Interval 1...X
	/// </summary>
	public class Interval: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override OperatorPriority Priority {get {return OperatorPriority.IntervalPriority;}}

		protected override object EvaluateII(int left, int right) {
            if(left < right) {
                TArray result = new TArray(typeof(int), right - left + 1);
                for(int i = left; i <= right; i++)
                    result[i - left] = i;
                return result;
            }
            else {
                TArray result = new TArray(typeof(int), left - right + 1);
                for(int i = left; i >= right; i--)
                    result[left - i] = i;
                return result;
            }
		}

		private const string operatorName = "...";
	}
}
