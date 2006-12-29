using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions
{
	/// <summary>
	/// Vrací rozmìry daného prvku jako øadu
	/// </summary>
	public class Length: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Vector) {
				TArray result = new TArray();
				result.Add((item as Vector).Length);
				return result;
			}
			else if(item is PointVector) {
				TArray result = new TArray();
				result.Add((item as PointVector).Length);
				return result;
			}
			else if(item is Matrix) {
				TArray result = new TArray();
				Matrix m = item as Matrix;

				result.Add(m.LengthX);
				result.Add(m.LengthY);

				return result;
			}
			else if(item is TArray) {
				TArray array = item as TArray;

				object result = null;
				if(array.Count > 0)
					result = this.Evaluate(depth + 1, array[0], arguments);
				if(!(result is TArray))
					result = new TArray();
				(result as TArray).Insert(0, array.Count);

				return result;
			}
			else
				return 0;
		}

		private const string help = "Vrací rozmìry objektu (poèty prvkù v jednotlivých dimenzích) jako øadu; pokud je objekt skalár, vrací 0";
		private const string parameters = "cokoliv";
	}

}
