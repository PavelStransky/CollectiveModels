using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions
{
	/// <summary>
	/// Vrac� rozm�ry dan�ho prvku jako �adu
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
				Array result = new Array();
				result.Add((item as Vector).Length);
				return result;
			}
			else if(item is PointVector) {
				Array result = new Array();
				result.Add((item as PointVector).Length);
				return result;
			}
			else if(item is Matrix) {
				Array result = new Array();
				Matrix m = item as Matrix;

				result.Add(m.LengthX);
				result.Add(m.LengthY);

				return result;
			}
			else if(item is Array) {
				Array array = item as Array;

				object result = null;
				if(array.Count > 0)
					result = this.Evaluate(depth + 1, array[0], arguments);
				if(!(result is Array))
					result = new Array();
				(result as Array).Insert(0, array.Count);

				return result;
			}
			else
				return 0;
		}

		private const string help = "Vrac� rozm�ry objektu (po�ty prvk� v jednotliv�ch dimenz�ch) jako �adu; pokud je objekt skal�r, vrac� 0";
		private const string parameters = "cokoliv";
	}

}
