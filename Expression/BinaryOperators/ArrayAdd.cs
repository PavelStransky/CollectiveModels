using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// P�id�n� prvku do �ady, resp. spojen� dvou �ad
	/// </summary>
	public class ArrayAdd: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return multiplePriority;}}

		public override object Evaluate(object left, object right) {
			TArray result = new TArray();

			if(left is TArray) {
				foreach(object item in (left as TArray))
					result.Add(item);

				if(right is TArray)
					foreach(object item in (right as TArray))
						result.Add(item);
				else
					result.Add(right);
			}
			else if(right is TArray) {
				result.Add(left);

				foreach(object item in (right as TArray))
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
