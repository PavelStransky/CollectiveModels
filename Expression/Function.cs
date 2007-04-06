using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Xml;

using PavelStransky.Math;

namespace PavelStransky.Expression
{
	/// <summary>
	/// Tøída pro vyhodnocení funkce
	/// </summary>
	public class Function: Atom {
		// Typ operátoru
		private Functions.FunctionDefinition function;

		// Argumenty funkce
		private ArrayList arguments = new ArrayList();

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Function(string expression, Atom parent)
            : base(expression, parent) { 
			int pos = FindOpenBracketPosition(this.expression);
            string fncName = this.expression.Substring(0, pos).Trim().ToLower();
            if(functions.Contains(fncName))
                this.function = functions[fncName];
            else if((this.function = Functions.UserFunction.CreateUserFunction(fncName)) == null)
                throw new ExpressionException(string.Format(errorMessageFunctionNotExists, fncName));
            
			string args = this.expression.Substring(pos + 1, this.expression.Length - pos - 2).Trim();
            
			if(args.Length == 0)
				return;

			string [] a = SplitArguments(args);
			for(int i = 0; i < a.Length; i++) {
				string arg = RemoveOutsideBracket(a[i]).Trim();
				this.arguments.Add(this.CreateAtomObject(arg));
			}
		}

    	/// <summary>
		/// Provede výpoèet funkce
		/// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <returns>Výsledek výpoètu</returns>
		public override object Evaluate(Guider guider) {
            object result = null;

            DateTime startTime = DateTime.Now; 			
			result = this.function.Evaluate(guider, this.arguments);
			return result;
		}

        private const string errorMessageFunctionNotExists = "Funkce {0} neexistuje.";
    }
}
