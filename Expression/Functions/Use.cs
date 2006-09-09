using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí øadu s použitím všech zaregistrovaných funkcí
	/// </summary>
	public class Use: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovník zaregistrovaných funkcí</param>
		public Use(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMaxNumber(arguments, 1);

			if(arguments.Count > 0) {
				if(arguments[0] is string)
					return functions[arguments[0] as string].Use;
				else
					return this.BadTypeError(arguments[0], 0);
			}
			else {
				Array result = new Array();

				foreach(FunctionDefinition f in functions.Values)
					result.Add(f.Use);

				return result;
			}
		}

		private const string help = "Pokud je zadán název funkce, vrátí její použití, jinak vytvoøí øadu (Array) s použitím všech zaregistrovaných funkcí";
		private const string parameters = "[název funkce]";
	}
}
