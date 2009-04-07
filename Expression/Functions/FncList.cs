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
					throw new FncListException(string.Format(Messages.EMBadFunctionName, functionName));
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
				throw new FncListException(string.Format(Messages.EMFunctionExists, functionName));
			this.Add(functionName, function);
		}
	}
}
