using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Pøidání prvku do øady, resp. spojení dvou øad
	/// </summary>
	public class ArrayAdd: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return multiplePriority;}}

		public override object Evaluate(object left, object right) {
			Array result = new Array();

			if(left is Array) {
				foreach(object item in (left as Array))
					result.Add(item);

				if(right is Array)
					foreach(object item in (right as Array))
						result.Add(item);
				else
					result.Add(right);
			}
			else if(right is Array) {
				result.Add(left);

				foreach(object item in (right as Array))
					result.Add(item);
			}
			else {
				result.Add(left);
				result.Add(right);
			}

			return result;
		}


		private const string operatorName = "&";
	}
}
