using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z argument� funkce vytvo�� �adu graf�
	/// </summary>
	public class FnGraphArray: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Use {get {return use;}}
		
		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			GraphArray result = new GraphArray();
			for(int i = 0; i < arguments.Count; i++) {
				this.CheckArgumentsType(arguments, i, typeof(Graph));
				result.Add(arguments[i]);
			}
			return result;
		}

		private const string name = "grapharray";
		private const string help = "Z argument� funkce vytvo�� �adu graf� (GraphArray)";
		private const string use = "grapharray([graf1; graf2; ...]);";
	}
}
