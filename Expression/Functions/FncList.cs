using System;
using System.Collections;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Zapouzdøuje všechny definované funkce do slovníku
	/// </summary>
	public class FncList: Hashtable {
		/// <summary>
		/// Vrací funkci podle jeho oznaèení
		/// </summary>
		public Fnc this[string functionName] {
			get {
				if(!this.Contains(functionName))
					throw new FunctionDefinitionException(string.Format(errorMessageBadFunctionName, functionName));
				else
					return base[functionName] as Fnc;
			}
		}

		/// <summary>
		/// Pøidá novou funkci do slovníku
		/// </summary>
		/// <param name="function">Funkce</param>
		public void Add(Fnc function) {
			string functionName = function.Name.ToLower();

			if(this.Contains(functionName))
				throw new FunctionDefinitionException(string.Format(errorMessageFunctionExists, functionName));
			this.Add(functionName, function);
		}

		private const string errorMessageBadFunctionName = "Neznámá funkce '{0}'.";
		private const string errorMessageFunctionExists = "Funkce '{0}' již ve slovníku funckcí existuje.";
	}
}
