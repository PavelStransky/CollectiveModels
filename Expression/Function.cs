using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Xml;

using PavelStransky.Math;

namespace PavelStransky.Expression
{
	/// <summary>
	/// T��da pro vyhodnocen� funkce
	/// </summary>
	public class Function: Atom {
		// Typ oper�toru
		private Functions.Fnc function;

		// Argumenty funkce
		private ArrayList arguments = new ArrayList();

        /// <summary>
        /// Jm�no funkce
        /// </summary>
        public string Name { get { return this.function.Name; } }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz funkce</param>
        /// <param name="parent">Rodi�</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
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
		/// Provede v�po�et funkce
		/// </summary>
        /// <param name="guider">Pr�vodce v�po�tu</param>
        /// <returns>V�sledek v�po�tu</returns>
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
