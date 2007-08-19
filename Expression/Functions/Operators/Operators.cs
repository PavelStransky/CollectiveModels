using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace PavelStransky.Expression.Operators {
	/// <summary>
	/// Zapouzd�uje oper�tory do slovn�ku
	/// </summary>
	public class Operators: Hashtable {
		private int maxOperatorLength = 0;

		/// <summary>
		/// Maxim�ln� d�lka oper�toru
		/// </summary>
		public int MaxOperatorLength {get {return this.maxOperatorLength;}}

		/// <summary>
		/// Vrac� bin�rn� oper�tor podle jeho ozna�en�
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
		/// P�id� nov� oper�tor do slovn�ku
		/// </summary>
		/// <param name="op">Oper�tor</param>
		public void Add(Operator op) {
			string operatorName = op.OperatorName;

			if(this.Contains(operatorName))
				throw new OperatorException(string.Format(errorMessageOperatorExists, operatorName));
			this.Add(operatorName, op);

			if(operatorName.Length > this.maxOperatorLength)
				this.maxOperatorLength = operatorName.Length;
		}

		/// <summary>
		/// Nahrad� v�echny oper�tory ve v�razu z�stupn�m znakem
		/// </summary>
		/// <param name="c">Z�stupn� znak</param>
		/// <param name="e">V�raz</param>
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

		private const string errorMessageBadOperator = "Nezn�m� oper�tor '{0}'.";
		private const string errorMessageOperatorExists = "Oper�tor '{0}' ji� ve slovn�ku oper�tor� existuje.";
	}
}
