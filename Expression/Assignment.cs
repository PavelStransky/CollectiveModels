using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// P�i�azen� ve v�razu, obsahuje rovn�tko (nap�. a = x + 1)
    /// </summary>
    public class Assignment: Atom {
        // Jm�no v�razu (to, co je na lev� stran� od rovn�tka)
        private string name;
        // Objekt, kter� p�i�azujeme
        private object rightSide;

        /// <summary>
        /// Jm�no v�razu (v�e, co je na lev� stran� od rovn�tka)
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz funkce</param>
        public Assignment(string expression, Atom parent)
            : this(expression, parent, null) {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz funkce</param>
        /// <param name="parent">Rodi�</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
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
        /// Provede v�po�et
        /// </summary>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        /// <returns>V�sledek v�po�tu</returns>
        public override object Evaluate(Context context) {
            object result = EvaluateAtomObject(context, this.rightSide);
            if(result != null)
                return context.SetVariable(this.name, result, this);
            else
                throw new ExpressionException(string.Format(errorMessageNullValue, this.name), string.Format(errorMessageNullValueDetail, this.expression));
        }

        private const string errorMessageNoSign = "Chyb� oper�tor p�i�azen� '='.";
        private const string errorMessageNoSignDetail = "��st v�razu: {0}";

        private const string errorMessageNullValue = "V�sledkem v�po�tu ��sti v�razu je hodnota NULL. Nelze dosadit do prom�nn� \"{0}\".";
        private const string errorMessageNullValueDetail = "��st v�razu: {0}";
    }
}
