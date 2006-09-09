using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Podm�nka If
	/// </summary>
	public class FnIf: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMinNumber(arguments, 2);
			this.CheckArgumentsMaxNumber(arguments, 3);

			object condition = Atom.EvaluateAtomObject(context, arguments[0]);
			if(!(condition is bool))
				this.BadTypeError(condition, 0);

			if((bool)condition)
				return Atom.EvaluateAtomObject(context, arguments[1]);
			else if(arguments.Count > 2)
				return Atom.EvaluateAtomObject(context, arguments[2]);
			else
				return null;
		}

		private const string errorMessageNoExpression = "Nelze vyhodnotit funkci {0}. Argument nen� prom�nn� nebo prom�nn� neobsahuje v�raz.";

		private const string name = "if";
		private const string help = "Podm�nka for";
		private const string parameters = "podm�nka; p��kaz spln�n�; p��kaz nespln�n�";
	}
}
