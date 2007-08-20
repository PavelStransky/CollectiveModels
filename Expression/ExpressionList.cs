using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;
/*
namespace PavelStransky.Expression {
	/// <summary>
	/// Tøída pro zpracování více pøíkazù oddìlených støedníkem (pøíkaz1; pøíkaz2; ...)
	/// </summary>
	public class ExpressionList: Atom {
		// Jednotlivé pøíkazy
		protected ArrayList commands = new ArrayList();

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz</param>
        /// <param name="parent">Rodiè</param>
        public ExpressionList(string expression, Atom parent)
            : base(expression, parent) {
            string[] c = SplitArguments(this.expression);

            for(int i = 0; i < c.Length; i++) {
                string command = RemoveOutsideBracket(c[i]);
                this.commands.Add(this.CreateAtomObject(command));
            }
        }

		/// <summary>
		/// Provede zpracování
		/// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <returns>Výsledek výpoètu je vždy null</returns>
		public override object Evaluate(Guider guider) {
			object retValue = null;

			for(int i = 0; i < this.commands.Count; i++) 
				retValue = EvaluateAtomObject(guider, this.commands[i]);
			
			return retValue;
		}
	}
}
*/