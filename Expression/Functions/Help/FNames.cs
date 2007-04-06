using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvo�� �adu s n�zvy v�ech zaregistrovan�ch funkc�
	/// </summary>
	public class FNames: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovn�k zaregistrovan�ch funkc�</param>
		public FNames(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return help;}}

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 0);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray result = new TArray(typeof(string), this.functions.Count);

            int i = 0;
			foreach(string fName in functions.Keys)
				result[i++] = fName;

			return result;
		}

		private const string help = "Vytvo�� �adu (Array) s n�zvy v�ech zaregistrovan�ch funkc�";
	}
}