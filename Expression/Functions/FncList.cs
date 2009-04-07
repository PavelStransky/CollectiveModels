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
					throw new FncListException(string.Format(Messages.EMBadFunctionName, functionName));
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
				throw new FncListException(string.Format(Messages.EMFunctionExists, functionName));
			this.Add(functionName, function);
		}
	}
}
