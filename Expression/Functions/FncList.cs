using System;
using System.Collections;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Zapouzd�uje v�echny definovan� funkce do slovn�ku
	/// </summary>
	public class FncList: Hashtable {
		/// <summary>
		/// Vrac� funkci podle jeho ozna�en�
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
		/// P�id� novou funkci do slovn�ku
		/// </summary>
		/// <param name="function">Funkce</param>
		public void Add(Fnc function) {
			string functionName = function.Name.ToLower();

			if(this.Contains(functionName))
				throw new FunctionDefinitionException(string.Format(errorMessageFunctionExists, functionName));
			this.Add(functionName, function);
		}

		private const string errorMessageBadFunctionName = "Nezn�m� funkce '{0}'.";
		private const string errorMessageFunctionExists = "Funkce '{0}' ji� ve slovn�ku funckc� existuje.";
	}
}
