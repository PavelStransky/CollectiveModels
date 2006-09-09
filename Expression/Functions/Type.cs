using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrac� typ dan�ho prvku jako �adu
	/// </summary>
	public class FnType: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Array) {
				Array array = item as Array;

				object result = null;
				if(array.Count > 0)
					result = this.Evaluate(depth + 1, array[0], arguments);
				if(!(result is Array))
					result = new Array();
				(result as Array).Insert(0, array.GetType().FullName);

				return result;
			}
			else {
				Array result = new Array();
				result.Add(item.GetType().FullName);
				return result;
			}
		}

		private const string name = "type";
		private const string help = "Vrac� typ objektu (typy jednotliv�ch dimenz�) jako �adu.";
		private const string parameters = "cokoliv";
	}

}
