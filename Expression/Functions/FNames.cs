using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí øadu s názvy všech zaregistrovaných funkcí
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

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 0);

			Array result = new Array();

			foreach(string fName in functions.Keys)
				result.Add(fName);

			return result;
		}

		private const string help = "Vytvoøí øadu (Array) s názvy všech zaregistrovaných funkcí";
	}
}
