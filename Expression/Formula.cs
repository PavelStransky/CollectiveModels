using System;
using System.Text.RegularExpressions;

using PavelStransky.Math;
using PavelStransky.Expression.BinaryOperators;

namespace PavelStransky.Expression {
	public class Formula: Atom {
		// Typ oper�toru
		private BinaryOperator binaryOperator;
		// Operandy
		private object leftOperand, rightOperand;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz</param>
        /// <param name="parent">Rodi�</param>
        public Formula(string expression, Atom parent)
            : base(expression, parent) {
            this.expression = FillBracket(this.expression);

            string operatorName = string.Empty;
            int pos = FindBinaryOperatorPosition(out operatorName, this.expression);
            if(pos < 0)
                throw new ExpressionException(string.Format(errorMessageNoOperator, this.expression));

            this.binaryOperator = binaryOperators[operatorName] as BinaryOperator;

            string left = RemoveOutsideBracket(this.expression.Substring(0, pos)).Trim();
            string right = RemoveOutsideBracket(this.expression.Substring(pos + operatorName.Length, this.expression.Length - pos - operatorName.Length)).Trim();

            this.leftOperand = this.CreateAtomObject(left);
            this.rightOperand = this.CreateAtomObject(right);
        }

		/// <summary>
		/// Provede v�po�et vzorce
		/// </summary>
        /// <param name="guider">Pr�vodce v�po�tem</param>
        /// <returns>V�sledek v�po�tu</returns>
		public override object Evaluate(Guider guider) {
			object left = EvaluateAtomObject(guider, this.leftOperand);
			object right = EvaluateAtomObject(guider, this.rightOperand);

			return this.binaryOperator.Evaluate(left, right);
		}

		/// <summary>
		/// Dopln� z�vorky do v�razu
		/// </summary>
		/// <param name="e">V�raz</param>
		private static string FillBracket(string e) {
            for(int i = (int)BinaryOperators.BinaryOperator.MaxPriority; i >= 0; i--) {
                MatchCollection matches = Regex.Matches(e, binaryOperators.OperatorPattern, RegexOptions.ExplicitCapture);

                int addIndex = 0;
                foreach(Match match in matches) {
                    if(!binaryOperators.Contains(match.Value))
                        continue;
                    if(IsInString(e, match.Index + addIndex))
                        continue;

                    if(((int)(binaryOperators[match.Value] as BinaryOperator).Priority) == i) {
                        string left = e.Substring(0, match.Index + addIndex);
                        if(!IsInBracket(left)) {
                            int index = match.Index + addIndex + match.Length;
                            e = AddOpenBracket(left) + match.Value + AddCloseBracket(e.Substring(index, e.Length - index));
                            addIndex += 2;
                        }
                    }
                }
            }

			return RemoveOutsideBracket(e);
		}

		/// <summary>
		/// P�id� k ��sti v�razu na prvn� vhodn� m�sto po��te�n� z�vorku
		/// </summary>
		/// <param name="e">��st v�razu</param>
		/// <returns>V�raz s p�idanou z�vorkou</returns>
		private static string AddOpenBracket(string e) {
			string subste = binaryOperators.SubstituteOperators(substitutionChar, e);

			int numBracket = 0;
			int i;

			for(i = e.Length - 1; i >= 0; i--) {
				if(IsInString(e, i))
					continue;

				if(e[i] == closeBracket || e[i] == closeIndexBracket) {
					numBracket++;
				}
				else if(e[i] == openBracket || e[i] == openIndexBracket) {
					if(numBracket == 0) 
						break;
					else
						numBracket--;
				}
				else if(numBracket == 0 && subste[i] == substitutionChar)
					break;
			}

			if(numBracket != 0)
				throw new ExpressionException(string.Format(errorMessageBracket, e));

			if(i <= 0)
				e = openBracket + e;
			else
				e = e.Insert(i + 1, openBracket.ToString());

			return e;
		}

		/// <summary>
		/// P�id� k ��sti v�razu na prvn� vhodn� m�sto koncovou z�vorku
		/// </summary>
		/// <param name="e">��st v�razu</param>
		/// <returns>V�raz s p�idanou z�vorkou</returns>
		private static string AddCloseBracket(string e) {
			string subste = binaryOperators.SubstituteOperators(substitutionChar, e);

			int numBracket = 0;
			int i;

			for(i = 0; i < e.Length; i++) {
				if(IsInString(e, i))
					continue;

				if(e[i] == openBracket || e[i] == openIndexBracket) {
					numBracket++;
				}
				else if(e[i] == closeBracket || e[i] == closeIndexBracket) {
					if(numBracket == 0) 
						break;
					else
						numBracket--;
				}
				else if(numBracket == 0 && subste[i] == substitutionChar)
					break;
			}

			if(numBracket != 0)
				throw new ContextException(string.Format(errorMessageBracket, e));

			if(i >= e.Length)
				e += closeBracket;
			else
				e = e.Insert(i, closeBracket.ToString());

			return e;
		}

		private const string errorMessageBracket = "V ��sti v�razu {0} nastala chyba ve zpracov�n� z�vorek.";
		private const string errorMessageNullValue = "Nelze vyhodnotit ��st v�razu.";
		private const string errorMessageNullValueDetail = "��st v�razu: {0}\nTyp prom�nn� 1: {1}\nTyp prom�nn� 2: {2}\nOper�tor: {3}";
		private const string errorMessageNoOperator = "V ��sti v�razu {0} nelze nal�zt oper�tor ke zpracov�n�.";
	}
}
