using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vr�t� n�pov�du k zadan� funkci
	/// </summary>
	public class FnHelp: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovn�k zaregistrovan�ch funkc�</param>
		public FnHelp(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 1);

			if(arguments[0] is string)
				return functions[arguments[0] as string].Help;
			else
				return this.BadTypeError(arguments[0], 0);
		}

		private const string name = "help";
		private const string help = "Vr�t� n�pov�du k zadan� funkci";
		private const string parameters = "n�zev funkce";
	}
}
