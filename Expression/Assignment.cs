using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Pøiøazení ve výrazu, obsahuje rovnítko (napø. a = x + 1)
    /// </summary>
    public class Assignment: Atom {
        // Levá strana výrazu (to, èemu bude pøiøazováno)
        private object leftSide;
        // Objekt, který pøiøazujeme
        private object rightSide;

        // True, pokud je na levé stranì výrazu indexer
        private bool isIndexer;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz funkce</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
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
        /// Delegát funkce, která provede pøiøazení
        /// </summary>
        public delegate object AssignmentFunction(object o);

        /// <summary>
        /// Pøiøazovací funkce
        /// </summary>
        /// <param name="o">Objekt pro pøiøazení</param>
        private object AssignFn(object o) {
            return this.result;
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <returns>Výsledek výpoètu</returns>
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

        private const string errorMessageNullValue = "Výsledkem výpoètu èásti výrazu je hodnota NULL. Nelze dosadit do promìnné.";
    }
}
