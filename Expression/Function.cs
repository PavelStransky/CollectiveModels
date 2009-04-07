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
		private Functions.Fnc function;

		// Argumenty funkce
		private ArrayList arguments = new ArrayList();

        /// <summary>
        /// Jméno funkce
        /// </summary>
        public string Name { get { return this.function.Name; } }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Function(string expression, Atom parent)
            : base(expression, parent) {
            ArrayList parts = GetArguments(this.expression);

            string fncName = parts[0] as string;
            if(functions.Contains(fncName))
                this.function = functions[fncName];

            else if((this.function = Functions.UserFnc.CreateUserFunction(fncName)) == null)
                throw new AtomException(expression, string.Format(errorMessageFunctionNotExists, fncName));

            foreach(string s in (parts[1] as ArrayList))
				this.arguments.Add(this.CreateAtomObject(s));
		}

    	/// <summary>
		/// Provede výpoèet funkce
		/// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <returns>Výsledek výpoètu</returns>
		public override object Evaluate(Guider guider) {
            object result = null;

            try {
                result = this.function.Evaluate(guider, this.arguments);
            }
            catch(Exception e) {
                throw e;
            }
			return result;
		}

        private const string errorMessageFunctionNotExists = "Funkce {0} neexistuje.";
    }
}
