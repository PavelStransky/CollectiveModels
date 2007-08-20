using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Podmínka If
	/// </summary>
	public class FnIf: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string ParametersHelp {get {return parameters;}}

        public override object Evaluate(Guider guider, ArrayList arguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(arguments, 2);
			this.CheckArgumentsMaxNumber(arguments, 3);

			object condition = Atom.EvaluateAtomObject(guider, arguments[0]);
			if(!(condition is bool))
				this.BadTypeError(condition, 0);

			if((bool)condition)
				return Atom.EvaluateAtomObject(guider, arguments[1]);
			else if(arguments.Count > 2)
				return Atom.EvaluateAtomObject(guider, arguments[2]);
			else
				return null;
		}

		private const string name = "if";
		private const string help = "Podmínka for";
		private const string parameters = "podmínka; pøíkaz splnìní; pøíkaz nesplnìní";
	}
}
