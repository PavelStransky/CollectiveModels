using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vr�t� n�pov�du k zadan� funkci (v�etn� u�it�)
	/// </summary>
	public class FullHelp: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovn�k zaregistrovan�ch funkc�</param>
		public FullHelp(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return functions[arguments[0] as string].FullHelp;
		}

		private const string help = "Vr�t� n�pov�du k zadan� funkci (v�etn� u�it�)";
		private const string parameters = "n�zev funkce";
	}
}
