using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrátí nápovìdu k zadané funkci
	/// </summary>
	public class FnHelp: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovník zaregistrovaných funkcí</param>
		public FnHelp(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return functions[arguments[0] as string].Help;
		}

		private const string name = "help";
		private const string help = "Vrátí nápovìdu k zadané funkci";
		private const string parameters = "název funkce";
	}
}
