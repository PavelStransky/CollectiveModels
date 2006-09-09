using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Cyklus for
	/// </summary>
	public class FnFor: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMinNumber(arguments, 3);
			this.CheckArgumentsMaxNumber(arguments, 4);

			Atom.EvaluateAtomObject(context, arguments[0]);

			object condition = Atom.EvaluateAtomObject(context, arguments[1]);
			if(!(condition is bool))
				this.BadTypeError(condition, 1);

			while((bool)condition) {
				Atom.EvaluateAtomObject(context, arguments[2]);

				condition = Atom.EvaluateAtomObject(context, arguments[1]);
				if(!(condition is bool))
					this.BadTypeError(condition, 1);				
			}

			if(arguments.Count < 4)
                return null;
			else
				return Atom.EvaluateAtomObject(context, arguments[3]);
		}

		private const string errorMessageNoExpression = "Nelze vyhodnotit funkci {0}. Argument není promìnná nebo promìnná neobsahuje výraz.";

		private const string name = "for";
		private const string help = "Cyklus for";
		private const string parameters = "inicializaèní pøíkaz; podmínka opakování; pøíkaz pøi každém provedení [; pøíkaz po ukonèení]";
	}
}
