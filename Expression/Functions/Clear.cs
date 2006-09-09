using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vyma�e prom�nnou z kontextu
	/// </summary>
	public class Clear: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMaxNumber(arguments, 1);

			if(arguments.Count == 0)
				context.Clear();
			else if(arguments[0] is string)
				context.Clear(arguments[0] as string);
			else
				throw new FunctionDefinitionException(string.Format(errorMessageNoVariable, this.Name));

			return null;
		}

		private const string errorMessageNoVariable = "Nelze dokon�it p��kaz. V argumentu funkce {0} je po�adov�na prom�nn�.";

		private const string help = "Vyma�e prom�nnou z kontextu";
		private const string parameters = "prom�nn�";
	}
}
