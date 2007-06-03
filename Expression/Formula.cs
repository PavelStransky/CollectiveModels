using System;
using System.Text.RegularExpressions;

using PavelStransky.Math;
using PavelStransky.Expression.BinaryOperators;

namespace PavelStransky.Expression {
	public class Formula: Atom {
		// Typ operátoru
		private BinaryOperator binaryOperator;
		// Operandy
		private object leftOperand, rightOperand;

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz</param>
        /// <param name="parent">Rodiè</param>
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
		/// Provede výpoèet vzorce
		/// </summary>
        /// <param name="guider">Prùvodce výpoètem</param>
        /// <returns>Výsledek výpoètu</returns>
		public override object Evaluate(Guider guider) {
			object left = EvaluateAtomObject(guider, this.leftOperand);
			object right = EvaluateAtomObject(guider, this.rightOperand);

			return this.binaryOperator.Evaluate(left, right);
		}

		/// <summary>
		/// Doplní závorky do výrazu
		/// </summary>
		/// <param name="e">Výraz</param>
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
		/// Pøidá k èásti výrazu na první vhodné místo poèáteèní závorku
		/// </summary>
		/// <param name="e">Èást výrazu</param>
		/// <returns>Výraz s pøidanou závorkou</returns>
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
		/// Pøidá k èásti výrazu na první vhodné místo koncovou závorku
		/// </summary>
		/// <param name="e">Èást výrazu</param>
		/// <returns>Výraz s pøidanou závorkou</returns>
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

		private const string errorMessageBracket = "V èásti výrazu {0} nastala chyba ve zpracování závorek.";
		private const string errorMessageNullValue = "Nelze vyhodnotit èást výrazu.";
		private const string errorMessageNullValueDetail = "Èást výrazu: {0}\nTyp promìnné 1: {1}\nTyp promìnné 2: {2}\nOperátor: {3}";
		private const string errorMessageNoOperator = "V èásti výrazu {0} nelze nalézt operátor ke zpracování.";
	}
}
