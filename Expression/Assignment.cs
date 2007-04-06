using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// P�i�azen� ve v�razu, obsahuje rovn�tko (nap�. a = x + 1)
    /// </summary>
    public class Assignment: Atom {
        // Lev� strana v�razu (to, �emu bude p�i�azov�no)
        private object leftSide;
        // Objekt, kter� p�i�azujeme
        private object rightSide;

        // True, pokud je na lev� stran� v�razu indexer
        private bool isIndexer;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz funkce</param>
        /// <param name="parent">Rodi�</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
        public Assignment(string expression, Atom parent)
            : base(expression, parent) {
            int pos = FindAssignmentOperatorPosition(this.expression);

            if(pos <= 0)
                throw new ExpressionException(errorMessageNoSign, string.Format(errorMessageDetail, this.expression));
            else {
                string ls = RemoveOutsideBracket(this.expression.Substring(0, pos)).Trim();
                switch(ExpressionType(ls)) {
                    case ExpressionTypes.Variable:
                        this.leftSide = ls;
                        break;

                    case ExpressionTypes.Indexer:
                        this.leftSide = new Indexer(ls, this);
                        break;

                    default:
                        throw new ExpressionException(string.Format(errorMessageBadAssignment, this.leftSide.GetType().FullName),
                            string.Format(errorMessageDetail, this.expression));
                }

                string rs = RemoveOutsideBracket(this.expression.Substring(pos + 1, this.expression.Length - pos - 1)).Trim();
                this.rightSide = this.CreateAtomObject(rs);
            }
        }

        private object result;

        /// <summary>
        /// Deleg�t funkce, kter� provede p�i�azen�
        /// </summary>
        public delegate object AssignmentFunction(object o);

        /// <summary>
        /// P�i�azovac� funkce
        /// </summary>
        /// <param name="o">Objekt pro p�i�azen�</param>
        private object AssignFn(object o) {
            return this.result;
        }

        /// <summary>
        /// Provede v�po�et
        /// </summary>
        /// <param name="guider">Pr�vodce v�po�tu</param>
        /// <returns>V�sledek v�po�tu</returns>
        public override object Evaluate(Guider guider) {
            this.result = EvaluateAtomObject(guider, this.rightSide);
            if(this.result != null) {
                if(this.leftSide is string)
                    return guider.Context.SetVariable(this.leftSide as string, this.result);
                else {
                    (this.leftSide as Indexer).Evaluate(guider, this.AssignFn);
                    return this.result;
                }
            }
            else
                throw new ExpressionException(errorMessageNullValue,
                    string.Format(errorMessageDetail, this.expression));
        }

        private const string errorMessageNoSign = "The operator '=' of assignment is missing.";
        private const string errorMessageDetail = "The part of the expression: {0}";

        private const string errorMessageBadAssignment = "It is possible to assign only to variable or indexer, not {0}.";

        private const string errorMessageNullValue = "V�sledkem v�po�tu ��sti v�razu je hodnota NULL. Nelze dosadit do prom�nn�.";
    }
}
