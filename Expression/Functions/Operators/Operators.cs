using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace PavelStransky.Expression.Operators {
	/// <summary>
	/// Zapouzdøuje operátory do slovníku
	/// </summary>
	public class Operators: Hashtable {
		private int maxOperatorLength = 0;

		/// <summary>
		/// Maximální délka operátoru
		/// </summary>
		public int MaxOperatorLength {get {return this.maxOperatorLength;}}

		/// <summary>
		/// Vrací binární operátor podle jeho oznaèení
		/// </summary>
		public Operator this[string operatorName] {
			get {
				if(!this.Contains(operatorName))
					throw new OperatorException(string.Format(errorMessageBadOperator, operatorName));
				else
					return base[operatorName] as Operator;
			}
		}

		/// <summary>
		/// Pøidá nový operátor do slovníku
		/// </summary>
		/// <param name="op">Operátor</param>
		public void Add(Operator op) {
			string operatorName = op.OperatorName;

			if(this.Contains(operatorName))
				throw new OperatorException(string.Format(errorMessageOperatorExists, operatorName));
			this.Add(operatorName, op);

			if(operatorName.Length > this.maxOperatorLength)
				this.maxOperatorLength = operatorName.Length;
		}

		/// <summary>
		/// Nahradí všechny operátory ve výrazu zástupným znakem
		/// </summary>
		/// <param name="c">Zástupný znak</param>
		/// <param name="e">Výraz</param>
        public string SubstituteOperators(char c, string e) {
            StringBuilder s = new StringBuilder(e);

            for(int i = this.maxOperatorLength; i >= 0; i--)
                foreach(string name in this.Keys) {
                    if(name.Length != i)
                        continue;

                    int index = -1;
                    while((index = e.IndexOf(name, index + 1)) >= 0) {
                        for(int j = 0; j < name.Length; j++)
                            s[index + j] = c;
                    }
                }

            return s.ToString();
        }

		private const string errorMessageBadOperator = "Neznámý operátor '{0}'.";
		private const string errorMessageOperatorExists = "Operátor '{0}' již ve slovníku operátorù existuje.";
	}
}
