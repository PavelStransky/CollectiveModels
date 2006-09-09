using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrátí nápovìdu k zadané funkci (vèetnì užití)
	/// </summary>
	public class FullHelp: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovník zaregistrovaných funkcí</param>
		public FullHelp(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 1);

			if(arguments[0] is string)
				return functions[arguments[0] as string].FullHelp;
			else
				return this.BadTypeError(arguments[0], 0);
		}

		private const string help = "Vrátí nápovìdu k zadané funkci (vèetnì užití)";
		private const string parameters = "název funkce";
	}
}
