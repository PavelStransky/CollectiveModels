using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Pøiøazení ve výrazu, obsahuje rovnítko (napø. a = x + 1)
    /// </summary>
    public class Assignment: Atom {
        // Jméno výrazu (to, co je na levé stranì od rovnítka)
        private string name;
        // Objekt, který pøiøazujeme
        private object rightSide;

        /// <summary>
        /// Jméno výrazu (vše, co je na levé stranì od rovnítka)
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz funkce</param>
        public Assignment(string expression, Atom parent)
            : this(expression, parent, null) {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz funkce</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Assignment(string expression, Atom parent, IOutputWriter writer)
            : base(expression, parent, writer) {
            int pos = FindAssignmentOperatorPosition(this.expression);

            if(pos <= 0)
                throw new ExpressionException(errorMessageNoSign, string.Format(errorMessageNoSignDetail, this.expression));
            else {
                this.name = this.expression.Substring(0, pos).Trim();
                string rs = RemoveOutsideBracket(this.expression.Substring(pos + 1, this.expression.Length - pos - 1)).Trim();
                this.rightSide = this.CreateAtomObject(rs);
            }
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        /// <returns>Výsledek výpoètu</returns>
        public override object Evaluate(Context context) {
            object result = EvaluateAtomObject(context, this.rightSide);
            if(result != null)
                return context.SetVariable(this.name, result, this);
            else
                throw new ExpressionException(string.Format(errorMessageNullValue, this.name), string.Format(errorMessageNullValueDetail, this.expression));
        }

        private const string errorMessageNoSign = "Chybí operátor pøiøazení '='.";
        private const string errorMessageNoSignDetail = "Èást výrazu: {0}";

        private const string errorMessageNullValue = "Výsledkem výpoètu èásti výrazu je hodnota NULL. Nelze dosadit do promìnné \"{0}\".";
        private const string errorMessageNullValueDetail = "Èást výrazu: {0}";
    }
}
