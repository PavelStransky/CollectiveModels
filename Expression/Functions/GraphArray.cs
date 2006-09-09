using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z argumentù funkce vytvoøí øadu grafù
	/// </summary>
	public class FnGraphArray: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			GraphArray result = new GraphArray();

			if(arguments.Count == 1 && arguments[0] is Array) {
				Array array = arguments[0] as Array;
				if(array.Count > 0 && array[0] as Graph == null)
					return this.BadTypeError(array[0], 0);

				for(int i = 0; i < array.Count; i++)
					result.Add(array[i]);
			}
			else {

				for(int i = 0; i < arguments.Count; i++) {
					if(arguments[i] as Graph != null)
						result.Add(arguments[i]);
					else
						return this.BadTypeError(arguments[i], i);
				}
			}

			return result;
		}

		private const string name = "grapharray";
		private const string help = "Z argumentù funkce vytvoøí øadu grafù (GraphArray)";
		private const string parameters = "[{graf1 [;graf2 [; ...]]} | {Array of Graph}]";
	}
}