using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Obsahuje - li prom�nn� v prvn�m parametru vzorec, p�epo��t� ji
	/// </summary>
	public class Eval: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 1);

			if(arguments[0] is string && context.Contains(arguments[0] as string)) {
				Variable v = context[arguments[0] as string];
				if(v.IsAssignment)
					return v.Evaluate();
			}
			else
				throw new FunctionDefinitionException(string.Format(errorMessageNoExpression, this.Name));

			return null;
		}

		private const string errorMessageNoExpression = "Nelze vyhodnotit funkci {0}. Argument nen� prom�nn� nebo prom�nn� neobsahuje v�raz.";

		private const string help = "P�epo��t� prom�nnou podle jej�ho vzorce";
		private const string parameters = "prom�nn�";
	}
}
