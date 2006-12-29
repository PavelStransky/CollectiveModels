using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z argumentù funkce vytvoøí øadu
	/// </summary>
	public class FnArray: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			TArray result = new TArray();
			for(int i = 0; i < arguments.Count; i++)
				result.Add(arguments[i]);
			return result;
		}

		private const string name = "array";
		private const string help = "Z argumentù funkce vytvoøí øadu (Array)";
		private const string parameters = "[prvek1 [;prvek2 [; ...]]]";
	}
}
