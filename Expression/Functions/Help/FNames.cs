using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
    /// Returns an array with names of all registered functions
	/// </summary>
	public class FNames: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovník zaregistrovaných funkcí</param>
		public FNames(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return help;}}

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 0);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray result = new TArray(typeof(string), this.functions.Count);

            int i = 0;
			foreach(string fName in functions.Keys)
				result[i++] = fName;

			return result;
		}

		private const string help = "Vytvoøí øadu (Array) s názvy všech zaregistrovaných funkcí";
	}
}