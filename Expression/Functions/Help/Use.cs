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

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments.Count > 0)
				return functions[arguments[0] as string].Use;
			else {
				TArray result = new TArray(typeof(string), functions.Count);

                int i = 0;
				foreach(FunctionDefinition f in functions.Values)
					result[i++] = f.Use;

				return result;
			}
		}

		private const string help = "Pokud je zadán název funkce, vrátí její použití, jinak vytvoøí øadu (Array) s použitím všech zaregistrovaných funkcí";
		private const string parameters = "[název funkce]";
	}
}
