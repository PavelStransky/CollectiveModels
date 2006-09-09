using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;

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
		/// <param name="context">Kontext</param>
		/// <param name="expression">Výraz</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public ExpressionList(Context context, string expression, Atom parent, IOutputWriter writer)
            : base(context, expression, parent, writer) {
            string[] c = SplitArguments(this.expression);

            for(int i = 0; i < c.Length; i++) {
                string command = RemoveOutsideBracket(c[i]);
                this.commands.Add(this.CreateAtomObject(command));
            }
        }

		/// <summary>
		/// Provede zpracování
		/// </summary>
		/// <returns>Výsledek výpoètu je vždy null</returns>
		public override object Evaluate() {
			object retValue = base.Evaluate();

			for(int i = 0; i < this.commands.Count; i++) 
				retValue = EvaluateAtomObject(this.context, this.commands[i]);
			
			return retValue;
		}
	}
}
