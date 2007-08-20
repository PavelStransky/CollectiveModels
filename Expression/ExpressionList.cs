using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;
/*
namespace PavelStransky.Expression {
	/// <summary>
	/// T��da pro zpracov�n� v�ce p��kaz� odd�len�ch st�edn�kem (p��kaz1; p��kaz2; ...)
	/// </summary>
	public class ExpressionList: Atom {
		// Jednotliv� p��kazy
		protected ArrayList commands = new ArrayList();

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz</param>
        /// <param name="parent">Rodi�</param>
        public ExpressionList(string expression, Atom parent)
            : base(expression, parent) {
            string[] c = SplitArguments(this.expression);

            for(int i = 0; i < c.Length; i++) {
                string command = RemoveOutsideBracket(c[i]);
                this.commands.Add(this.CreateAtomObject(command));
            }
        }

		/// <summary>
		/// Provede zpracov�n�
		/// </summary>
        /// <param name="guider">Pr�vodce v�po�tu</param>
        /// <returns>V�sledek v�po�tu je v�dy null</returns>
		public override object Evaluate(Guider guider) {
			object retValue = null;

			for(int i = 0; i < this.commands.Count; i++) 
				retValue = EvaluateAtomObject(guider, this.commands[i]);
			
			return retValue;
		}
	}
}
*/