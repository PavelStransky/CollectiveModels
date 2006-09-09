using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí èasovaè a spustí jej
	/// </summary>
	public class FnTimer: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 2);

			object interval = Atom.EvaluateAtomObject(context, arguments[1]);
			if(!(interval is int))
				this.BadTypeError(interval, 1);

			if(arguments[0] is Atom)
				return new Timer(arguments[0] as Atom, (int)interval);
			else
				return this.BadTypeError(arguments[0], 0);
		}

		private const string name = "timer";
		private const string help = "Vytvoøí èasovaè a spustí jej";
		private const string parameters = "výraz k opakovanému výpoètu; interval (int)";
	}
}
