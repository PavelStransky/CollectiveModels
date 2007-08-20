using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using PavelStransky.Math;
using PavelStransky.Expression.Functions;

namespace PavelStransky.Expression {
	/// <summary>
	/// Základní pøedek pro vzorec a funkci
	/// </summary>
    public abstract partial class Atom {
		/// <summary>
		/// Typy výrazù
		/// </summary>
		protected enum ExpressionTypes {
			Transform,
			Formula, 
			Function, 
			Null, 
			Bool, 
			Int32, 
			Double, 
			String, 
			Variable, 
			Indexer
		}

		// Náš výraz
		protected string expression;
		// Atom o úroveò výš (kvùli poèítání poètu krokù)
		protected Atom parent;

		// Promìnné, které se vyskytují v èásti výrazu
		protected ArrayList variables = new ArrayList();

        /// <summary>
        /// Rodiè
        /// </summary>
        public Atom Parent { get { return this.parent; } }

		/// <summary>
		/// Náš výraz
		/// </summary>
		public string Expression {get {return this.expression;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
		/// <param name="parent">Rodiè</param>
		public Atom(string expression, Atom parent) {
            this.parent = parent;
            this.expression = expression;
        }

        /// <summary>
        /// Provede výpoèet èásti výrazu
        /// </summary>
        /// <param name="context">Kontext</param>
        public object Evaluate(Context context) {
            return this.Evaluate(new Guider(context));
        }

		/// <summary>
		/// Provede výpoèet èásti výrazu
		/// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <returns>Výsledek výpoètu</returns>
		public virtual object Evaluate(Guider guider) {
			return null;
        }

        #region Kontrola Syntaxe
        /// <summary>
        /// Pozice v syntaxi
        /// </summary>
        private enum SyntaxPosition {
            Beginning, AfterOperator, AfterValue, AfterVariableOrFunction
        }

		/// <summary>
		/// Zkontroluje, zda je správnì zadaná syntaxe (závorky, uvozovky, operátory, funkce)
		/// </summary>
		/// <param name="e">Výraz</param>
        /// <returns>Polohu místa, kde je chyba</returns>
        public static void CheckSyntax(string e) {
            int length = e.Length;
            int n = openBracketChars.Length;
            ArrayList numBrackets = new ArrayList();

            // Aktuální poèet závorek; na nb[n] je pozice otevírací závorky
            int[] nb = new int[n + 1];
            nb[n] = -1;
            numBrackets.Add(nb);

            SyntaxPosition position = SyntaxPosition.Beginning;

            // Pomocná promìnná
            int j = 0;
            int i = 0;

            while(i < length) {
                if(noMeanChars.IndexOf(e[i]) >= 0) {
                    i++;
                    continue;
                }

                else if(e[i] == separatorChar) {
                    i++;
                    position = SyntaxPosition.Beginning;
                    continue;
                }

                // Cyklus pøes všechny závorky
                bool bracket = false;
                for(int k = 0; k < n; k++) {
                    if(e[i] == openBracketChars[k]) {
                        if(!CheckPositionType(position,         // Normální závorka
                            SyntaxPosition.Beginning, SyntaxPosition.AfterOperator,
                            SyntaxPosition.AfterVariableOrFunction) 
                            && k == 0)
                            throw new ExpressionException(
                                Messages.EMInvalidBracketPosition, 
                                string.Format(Messages.EMInvalidBracketPositionDetail, openBracketChars[k]), 
                                i);

                        else if(!CheckPositionType(position,    // Indexovací závorka
                            SyntaxPosition.AfterValue, SyntaxPosition.AfterVariableOrFunction)
                            && k == 1)

                        nb = (int[])numBrackets[0];
                        nb = (int[])nb.Clone();

                        nb[k]++;
                        nb[n] = i;

                        numBrackets.Insert(0, nb);

                        bracket = true;
                        position = SyntaxPosition.Beginning;
                    }

                    else if(e[i] == closeBracketChars[k]) {
                        if(CheckPositionType(position, SyntaxPosition.AfterOperator))
                            throw new ExpressionException(
                                Messages.EMInvalidBracketPosition, 
                                string.Format(Messages.EMInvalidBracketPositionDetail, closeBracketChars[k]), 
                                i);

                        if(numBrackets.Count == 1)
                            throw new ExpressionException(
                                Messages.EMInvalidBracketPosition, 
                                string.Format(Messages.EMInvalidBracketPositionDetailCloseBeforeOpen, closeBracketChars[k], openBracketChars[k]), 
                                i);

                        nb = (int[])numBrackets[0];
                        nb = (int[])nb.Clone();
                        nb[k]--;

                        int[] nbp = (int[])numBrackets[1];

                        for(int l = 0; l < n; l++)
                            if(nb[l] != nbp[l])
                                throw new ExpressionException(
                                    Messages.EMInvalidBracketPosition,
                                    string.Format(Messages.EMInvalidBracketPositionDetailBracketMixing, openBracketChars[(k + 1) % 2], closeBracketChars[k]),
                                    i, nbp[n]);

                        numBrackets.RemoveAt(0);
                        bracket = true;
                        position = SyntaxPosition.AfterValue;
                    }
                }

                if(bracket) {
                    i++;
                }

                else if(e[i] == endVariableChar) {
                    if(((int[])numBrackets[0])[1] <= 0)
                        throw new ExpressionException(
                            Messages.EMInvalidEndVariablePosition,
                            i);
                    i++;
                    position = SyntaxPosition.AfterValue;
                }

                else if((j = CommentPosition(e, i)) > 0) { // Komentáø pøeskoèíme
                    i = j;
                }

                else if((j = StringPosition(e, i)) > 0) {
                    if(!CheckPositionType(position,
                        SyntaxPosition.Beginning, SyntaxPosition.AfterOperator))
                        throw new ExpressionException(
                            string.Format(Messages.EMInvalidPosition, "String"),
                            i);

                    i = j;
                    position = SyntaxPosition.AfterValue;
                }

                else if((j = VariableOrFunctionPosition(e, i)) > 0) {
                    if(!CheckPositionType(position,
                        SyntaxPosition.Beginning, SyntaxPosition.AfterOperator))
                        throw new ExpressionException(
                            string.Format(Messages.EMInvalidPosition, "Variable"), 
                            i);

                    i = j;
                    position = SyntaxPosition.AfterVariableOrFunction;
                }

                else if((j = NumberPosition(e, i)) > 0) {
                    if(!CheckPositionType(position,
                        SyntaxPosition.Beginning, SyntaxPosition.AfterOperator))
                        throw new ExpressionException(
                            string.Format(Messages.EMInvalidPosition, "Number"),
                            i);

                    i = j;
                    position = SyntaxPosition.AfterValue;
                }

                else if((j = OperatorPosition(e, i)) > 0) {
                    i = j;
                    position = SyntaxPosition.AfterOperator;
                }

                else
                    throw new ExpressionException(Messages.EMInvalidCharacter, i);
            }

            if(CheckPositionType(position, SyntaxPosition.AfterOperator))
                throw new ExpressionException(Messages.EMInvalidExpressionEnd, i);
        }

        /// <summary>
        /// Zkontroluje, zda pozice je zadaného typu
        /// </summary>
        /// <param name="position">Aktuální pozice</param>
        /// <param name="types">Možné typy</param>
        /// <returns></returns>
        private static bool CheckPositionType(SyntaxPosition position, params SyntaxPosition[] types) {
            foreach(SyntaxPosition p in types)
                if(p == position)
                    return true;

            return false;
        }
        #endregion

        #region Pozice rùzných èástí výrazù
        /// <summary>
        /// Pozice dalšího znaku po komentáøi (pokud komentáø není, vrací -1)
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int CommentPosition(string e, int i) {
            if(e.IndexOf(commentChars, i) != i)
                return -1;

            int j = e.IndexOf('\n', i);
            if(j < 0)
                j = e.IndexOf('\r', i);
            if(j < 0)
                j = e.Length;

            return j;
        }

        /// <summary>
        /// Pozice dalšího znaku po øetìzci (pokud øetìzec není, vrací -1)
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int StringPosition(string e, int i) {
            if(e[i] != stringChar)
                return -1;

            int j = i;

            while((j = e.IndexOf(stringChar, j + 1)) >= 0)
                if(e[j - 1] != specialChar)
                    break;

            if(j < 0)
                throw new ExpressionException(Messages.EMStringCharMissing, i);

            return j + 1;
        }

        /// <summary>
        /// Pozice dalšího znaku po konci textu (pokud text není, vrací -1)
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int VariableOrFunctionPosition(string e, int i) {
            if(!char.IsLetter(e[i]) && e[i] != endVariableChar && variableChars.IndexOf(e[i]) < 0)
                return -1;

            int length = e.Length;
            while(++i < length && (char.IsLetterOrDigit(e[i]) || variableChars.IndexOf(e[i]) >= 0));

            return i;
        }

        /// <summary>
        /// Pozice dalšího znaku po èíslu (pokud èíslo není, vrací -1)
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int NumberPosition(string e, int i) {
            bool point = false;
            bool exp = false;

            if(e[i] == '.')
                point = true;
            else if(!char.IsDigit(e[i]))
                return -1;

            int length = e.Length;
            while(++i < length) {
                if(char.IsDigit(e[i]))
                    continue;

                else if(e[i] == '.') {
                    if(point || exp)
                        break;
                    else
                        point = true;
                }

                else if(char.ToUpper(e[i]) == 'E')
                    exp = true;

                else if(e[i] == '+' || e[i] == '-') {
                    if(!exp || char.ToUpper(e[i - 1]) != 'E')
                        break;
                }

                else
                    break;
            }

            return i;
        }

        /// <summary>
        /// Pozice dalšího znaku po operátoru (pokud operátor není, vrací -1)
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int OperatorPosition(string e, int i) {
            int j = FirstNonOperatorChar(e, i);

            if(i == j)
                return -1;

            string o = string.Empty;

            while((o = e.Substring(i, j - i)).Length > 0) {
                foreach(string name in functions.Keys)
                    if(name == o)
                        return j;
                j--;
            }

            return -1;
        }

        /// <summary>
        /// Vrátí pozici prvního znaku, který není operátorem
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int FirstNonOperatorChar(string e, int i) {
            int length = e.Length;
            int n = openBracketChars.Length;

            bool bracket = false;

            while(i < length) {
                if(noMeanChars.IndexOf(e[i]) >= 0)
                    break;
                else if(char.IsLetterOrDigit(e[i]))
                    break;
                else if(e[i] == '.')
                    break;
                else if(variableChars.IndexOf(e[i]) >= 0)
                    break;
                else if(e[i] == stringChar)
                    break;


                for(int k = 0; k < n; k++)
                    if(e[i] == openBracketChars[k] || e[i] == closeBracketChars[k]) {
                        bracket = true;
                        break;
                    }

                if(bracket)
                    break;

                i++;
            }

            return i;
        }

        /// <summary>
        /// Pozice dalšího znaku po závorce (pokud závorka není, vrací -1)
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="i">Poèáteèní pozice</param>
        private static int BracketPosition(string e, int i) {
            int length = e.Length;
            int n = openBracketChars.Length;

            int k = 0;
            while(k < n && e[i] != openBracketChars[k])
                k++;

            if(k == n)
                return -1;

            int j = 0;
            int numBrackets = 0;

            while(i < length) {
                if(e[i] == openBracketChars[k]) {
                    numBrackets++;
                    i++;
                }

                else if(e[i] == closeBracketChars[k]) {
                    numBrackets--;
                    if(numBrackets == 0)
                        return i + 1;
                    i++;
                }

                else if((j = StringPosition(e, i)) > 0)
                    i = j;

                else
                    i++;
            }

            return -1;
        }
        #endregion

        /// <summary>
        /// Odstraní komentáø a všechny znaky bez smyslu
        /// </summary>
        /// <param name="e">Výraz</param>
        protected static string RemoveComments(string e) {
            int length = e.Length;

            int i = 0;
            int j = 0;

            StringBuilder result = new StringBuilder(length);

            while(i < length) {
                if((j = CommentPosition(e, i)) > 0) // Pøeskoèíme komentáøe
                    i = j;

                else if((noMeanChars.IndexOf(e[i])) >= 0) {
                    if(i > 0 && result[result.Length - 1] != ' ')
                        result.Append(' ');
                    i++;
                }

                else if((j = StringPosition(e, i)) > 0) { // Pøeskoèíme a uložíme øetìzce
                    result.Append(e.Substring(i, j - i));
                    i = j;
                }

                else {
                    result.Append(e[i]);
                    i++;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Odstraní z konce všechny znaky komentáøe
        /// </summary>
        /// <param name="e">Výraz</param>
        protected static string RemoveEndSeparator(string e) {
            int i = e.Length - 1;
            
            while(i >= 0 && e[i] == separatorChar)
                i--;

            return e.Substring(0, i + 1);
        }

        /// <summary>
        /// Odstraní všechny vnìjší závorky z výrazu
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <returns>Výraz bez vnìjších závorek</returns>
        protected static string RemoveOutsideBracket(string e) {
			return RemoveOutsideBracket(0, e);
		}

		/// <summary>
		/// Odstraní všechny vnìjší závorky z výrazu
		/// </summary>
		/// <param name="e">Výraz</param>
		/// <returns>Výraz bez vnìjších závorek</returns>
		protected static string RemoveOutsideIndexBracket(string e) {
			return RemoveOutsideBracket(1, e);
		}

		/// <summary>
		/// Odstraní všechny vnìjší závorky z výrazu
		/// </summary>
		/// <param name="bracketNumber">Èíslo (typ) závorky</param>
		/// <param name="e">Výraz</param>
		/// <returns>Výraz bez vnìjších závorek</returns>
		private static string RemoveOutsideBracket(int bracketNumber, string e) {
			e = e.Trim();

			while(e.Length != 0) {
                int j = 0;
                if((j = BracketPosition(e, 0)) > 0 && e[0] == openBracketChars[bracketNumber] && j == e.Length)
                        e = e.Substring(1, e.Length - 2).Trim();
                else
                    break;
			}

            return e;
		}

		/// <summary>
		/// Zjistí, jakého typu je vstupní èást vzorce
		/// </summary>
		/// <param name="e">Výraz</param>
		protected static ExpressionTypes ExpressionType(string e) {
            e = e.Trim();
            int length = e.Length;

            int i = 0;
            int j = 0;

            ExpressionTypes result = ExpressionTypes.Null;

            while(i < length) {
                    // Operátor
                if((j = OperatorPosition(e, i)) > 0) {
                    result = ExpressionTypes.Function;
                    break;
                }

                    // Øetìzec
                else if((j = StringPosition(e, i)) > 0) {
                    result = ExpressionTypes.String;
                    break;
                }

                    // Promìnná
                else if((j = VariableOrFunctionPosition(e, i)) > 0) {
                    if(j == length) {
                        result = ExpressionTypes.Variable;
                        break;
                    }

                    i = j;
                }

                    // Èíslo
                else if((j = NumberPosition(e, i)) > 0) {
                    if(IsIntNumber(e.Substring(i, j - i)))
                        result = ExpressionTypes.Int32;
                    else
                        result = ExpressionTypes.Double;
                    break;
                }

                else if((j = BracketPosition(e, i)) > 0) {
                    if(e[i] == openBracketChars[0])
                        result = ExpressionTypes.Function;
                    else
                        result = ExpressionTypes.Indexer;
                    break;
                }

                else
                    i++;
            }

            return result;
		}

        /// <summary>
        /// Vrátí TRUE, pokud je èíslo celé
        /// </summary>
        /// <param name="e">Výraz obsahující pouze èíslo</param>
        private static bool IsIntNumber(string e) {
            e = e.ToUpper();
            if(e.IndexOf('.') >= 0 || e.IndexOf('E') >= 0)
                return false;
            else
                return true;
        }

		/// <summary>
		/// Rozhodne, zda výraz je logická promìnná (true, false)
		/// </summary>
		/// <param name="e">Výraz</param>
		protected static bool IsBool(string e) {
			if(e == boolTrue || e == boolFalse)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Rozhodne, jaký objekt z èásti výrazu vytvoøí
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
		/// <returns>Vytvoøený objekt</returns>
		protected object CreateAtomObject(string expression) {
			object retValue = null;

			switch(ExpressionType(expression)) {
				case ExpressionTypes.Function:
					retValue = new Function(expression, this);
					break;
				case ExpressionTypes.Indexer:
					retValue = new Indexer(expression, this);
					break;
				case ExpressionTypes.Int32:
					retValue = Convert.ToInt32(expression);
					break;
				case ExpressionTypes.Double:
					retValue = Convert.ToDouble(expression);
					break;
				case ExpressionTypes.String:
					retValue = expression;
					break;
				case ExpressionTypes.Bool:
					retValue = ParseBool(expression);
					break;
				case ExpressionTypes.Variable:
					this.AddVariableToList(expression);
					retValue = expression;
					break;
				case ExpressionTypes.Null:
					retValue = null;
					break;
			}

			// Do svého seznamu promìnných pøidáme všechny promìnné z nižší èásti výrazu
			if(retValue is Atom)
				this.AddVariablesToList((retValue as Atom).variables);

			return retValue;
		}

		/// <summary>
		/// Zjistí, zda výraz není uživatelem definovaný øetìzec (je v uvozovkách)
		/// </summary>
		/// <param name="e">Výraz</param>
		/// <returns>Je - li øetezec, pak øetìzec, jinak null</returns>
		protected static string ParseString(string e) {
			if(StringPosition(e,0)==e.Length)
				return e.Substring(1, e.Length - 2).Replace(specialChar.ToString() + stringChar.ToString(), stringChar.ToString());
			else
				return null;
		}

        /// <summary>
        /// Podle øetìzce urèí typ
        /// </summary>
        /// <param name="p">String s typem</param>
        internal static Type ParseType(string p) {
            p = p.Trim().ToLower();
            if(p == "string")
                return typeof(string);
            else if(p == "int")
                return typeof(int);
            else if(p == "double")
                return typeof(double);
            else if(p == "vector")
                return typeof(Vector);
            else if(p == "pointvector")
                return typeof(PointVector);
            else if(p == "array")
                return typeof(TArray);
            else if(p == "graph")
                return typeof(Graph);
            else if(p == "list")
                return typeof(List);
            else if(p == "matrix")
                return typeof(Matrix);

            throw new ExpressionException(string.Format(Messages.EMBadTypeName, p));
        }

		/// <summary>
		/// Pøevede logickou promìnnou zadanou v textovém øetìzci na typ bool
		/// </summary>
		/// <param name="e">Výraz</param>
		protected static bool ParseBool(string e) {
			if(e == boolTrue)
				return true;
			else if(e == boolFalse)
				return false;
			else
				throw new ExpressionException(Messages.EMBadBoolValue);
		}

		/// <summary>
		/// Pøidá názvy promìnných do vlastního seznamu promìnných
		/// </summary>
		/// <param name="variable">Názvy promìnných vnitøní èásti výrazu</param>
		/// <returns>Poèet pøidaných promìnných</returns>
		protected int AddVariablesToList(ArrayList variables) {
			int newCount = 0;

			foreach(string variable in variables) {
				if(!this.variables.Contains(variable)) {
					this.variables.Add(variable);
					newCount++;
				}
			}

			return newCount;
		}

		/// <summary>
		/// Pøidá název promìnné do seznamu promìnných
		/// </summary>
		/// <param name="variable">Název promìnné</param>
		/// <returns>Aktuální index</returns>
		protected int AddVariableToList(string variable) {
			if(!this.variables.Contains(variable))
				return this.variables.Add(variable);
			else
				return this.variables.IndexOf(variable);
		}

		/// <summary>
		/// Provede výpoèet dané èásti výrazu
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
		/// <param name="guider">Prùvodce výpoètu</param>
		/// <returns>Objekt získaný výpoètem</returns>
		public static object EvaluateAtomObject(Guider guider, object operand) {
			object retValue;

			if(operand is Atom) {
				retValue = (operand as Atom).Evaluate(guider);
			}
			else if(operand is string) {
				string s = ParseString(operand as String);
				if(s == null)
					retValue = guider.Context[operand as string];
				else
					retValue = s;
			}
			else {
				retValue = operand;
			}

			if(retValue is Variable)
				retValue = (retValue as Variable).Item;

			return retValue;
		}

        /// <summary>
        /// Doplní závorky do èásti výrazu
        /// </summary>
        /// <param name="e">Výraz</param>
        protected static string FillBracket(string e) {
            e = RemoveOutsideBracket(e);
            int length = e.Length;

            // Nejprve rozdìlíme
            ArrayList parts = new ArrayList();

            int i = 0;
            int j = 0;
            int k = 0;

            SyntaxPosition position = SyntaxPosition.Beginning;

            while(i < length) {
                if((j = StringPosition(e, i)) > 0 ||
                    (j = NumberPosition(e, i)) > 0 ||
                    (j = BracketPosition(e, i)) > 0) {
                    i = j;
                    position = SyntaxPosition.AfterValue;
                }

                else if((j = VariableOrFunctionPosition(e, i)) > 0) {
                    i = j;
                    position = SyntaxPosition.AfterVariableOrFunction;
                }

                else if((j = OperatorPosition(e, i)) > 0) {
                    if(CheckPositionType(position, SyntaxPosition.Beginning, SyntaxPosition.AfterOperator)) {
                        int l = j;
                        while(noMeanChars.IndexOf(e[l]) >= 0)
                            l++;

                        // Máme napøíklad toto: y + -(x; 12);
                        // - už je hotová funkce, nemusíme ji nijak zpracovávat
                        if(e[l] == openBracketChars[0]) {
                            i = BracketPosition(e, l);
                            position = SyntaxPosition.AfterVariableOrFunction;
                            continue;
                        }
                    }
                    else
                        parts.Add(e.Substring(k, i - k).Trim());

                    parts.Add(functions[e.Substring(i, j - i)]);
                    k = j;
                    i = j;
                    position = SyntaxPosition.AfterOperator;
                }

                else
                    i++;
            }

            parts.Add(e.Substring(k, length - k));

            // Rozdìleno, nyní hledáme vždy operátory od nejvyšší priority k nejnižší a vkládáme závorky
            // (unární operátory mají vždy vyšší prioritu než binární, proto je zpracujeme nejdøíve)
            ArrayList newParts = new ArrayList();

            int count = parts.Count;

            i = 0;
            while(i < count) {
                if(parts[i] is Operator && (parts[i + 1] is Operator || i == 0)) {
                    if(i != 0) 
                        newParts.Add(parts[i++]);

                    StringBuilder s = new StringBuilder();
                    j = 0;

                    while(parts[i] is Operator) {
                        s.Append((parts[i] as Operator).Name);
                        s.Append('(');
                        i++; j++;
                    }

                    s.Append(parts[i]);
                    s.Append(')', j);

                    newParts.Add(s.ToString());
                    i++;
                }
                else
                    newParts.Add(parts[i++]);
            }

            parts = newParts;

            for(k = (int)Operator.MaxPriority; k >= 0; k--) {
                count = parts.Count;
                if(count <= 1)
                    break;

                newParts = new ArrayList();
                object last = parts[count - 1];

                for(i = count - 2; i >= 0; i--) {
                    if(parts[i] is Operator && ((int)(parts[i] as Operator).Priority == k)) {
                        string name = (parts[i] as Operator).Name;
                        int numParams = (parts[i] as Operator).NumParams;

                        StringBuilder s = new StringBuilder();
                        i--;
                        s.Append(parts[i]);
                        i--;

                        j = 2;

                        while(i >= 0 && j < numParams) {
                            if((parts[i] is Operator) && (parts[i] as Operator).Name == name) {
                                s.Insert(0, separatorChar);
                                i--;
                                s.Insert(0, parts[i]);
                                i--;
                                j++;
                            }
                            else 
                                break;
                        }

                        i++;

                        s.Insert(0, '(');
                        s.Insert(0, name);
                        s.Append(separatorChar);
                        s.Append(last);
                        s.Append(')');
                        last = s.ToString();
                    }
                    else {
                        newParts.Insert(0, last);
                        last = parts[i];
                    }
                }

                newParts.Insert(0, last);
                parts = newParts;
            }

            return parts[0].ToString();
        }

        /// <summary>
        /// Provede rozdìlení argumentù funkce (indexù indexeru)
        /// </summary>
        /// <param name="e">Výraz</param>
        private static ArrayList SeparateArguments(string e) {
            int length = e.Length;

            int i = 0;
            int j = 0;
            int k = 0;

            ArrayList result = new ArrayList();

            while(i < length) {
                if((j = StringPosition(e, i)) > 0)
                    i = j;
                else if((j = BracketPosition(e, i)) > 0)
                    i = j;
                else if(e[i] == separatorChar) {
                    result.Add(FillBracket(e.Substring(k, i - k)));
                    k = i + 1;
                    i++;
                }
                else
                    i++;
            }

            if(length - k > 0)
                result.Add(FillBracket(e.Substring(k, length - k)));
            return result;
        }

        /// <summary>
        /// Oddìlí jméno funkce a její argumenty 
        /// </summary>
        /// <param name="e">Výraz</param>
        protected static ArrayList GetArguments(string e) {
            int length = e.Length;

            int i = 0;
            int j = 0;

            while(i < length) {
                if((j = StringPosition(e, i)) > 0)
                    i = j;
                else if((j = BracketPosition(e, i)) > 0) {
                    if(e[i] == openBracketChars[0])
                        break;
                    else
                        i++;
                }
                else
                    i++;
            }

            ArrayList result = new ArrayList();
            result.Add(e.Substring(0, i).Trim());
            result.Add(SeparateArguments(e.Substring(i + 1, length - i - 2)));
            return result;
        }

        /// <summary>
        /// Najde poslední indexaèní závorky a vrátí indexovaný výraz a indexy
        /// </summary>
        /// <param name="e">Výraz</param>
        protected static ArrayList GetIndexes(string e) {
            int length = e.Length;

            int i = 0;
            int j = 0;
            int k = 0;

            while(i < length) {
                if((j = StringPosition(e, i)) > 0)
                    i = j;
                else if((j = BracketPosition(e, i)) > 0) {
                    if(e[i] == openBracketChars[1])
                        k = i;
                    i = j;
                }
                else
                    i++;
            }

            ArrayList result = new ArrayList();
            result.Add(e.Substring(0, k).Trim());

            // Nahrazení koncových promìnných
            ArrayList indexes = SeparateArguments(e.Substring(k + 1, length - k - 2));
            ArrayList endVariables = new ArrayList();
            ArrayList indexesev = new ArrayList();

            foreach(string s in indexes) {
                string ev = GenerateEndVariable();
                endVariables.Add(ev);
                indexesev.Add(ReplaceEndVariables(s, ev));
            }

            result.Add(indexesev);
            result.Add(endVariables);

            return result;
        }

        /// <summary>
        /// Nahradí všechny znaky pro koncové promìnné daným oznaèením koncové promìnné
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="ev">Znaèení koncové promìnné</param>
        private static string ReplaceEndVariables(string e, string ev) {
            int length = e.Length;
            StringBuilder result = new StringBuilder();

            int i = 0;
            int j = 0;
            int k = 0;

            while(i < length) {
                if((j = StringPosition(e, i)) > 0)
                    i = j;
                else if((j = BracketPosition(e, i)) > 0) {
                    if(e[i] == openBracketChars[1])    // Pøeskakujeme jen další vnoøený indexer
                        i = j;
                    else
                        i++;
                }
                else if(e[i] == '$') {
                    result.Append(e.Substring(k, i));
                    result.Append(ev);
                    k = i + 1;
                    i++;
                }
                else
                    i++;
            }

            result.Append(e.Substring(k, length - k));

            return result.ToString();
        }

        private static int countEndVariables = 0;

        /// <summary>
        /// Nageneruje jedineènou promìnnou
        /// </summary>
        /// <returns></returns>
        private static string GenerateEndVariable() {
            return string.Format("$end{0}", countEndVariables++);
        }

        /// <summary>
        /// Delegát funkce, která provede pøiøazení
        /// </summary>
        public delegate object AssignmentFunction(object o);

        /// <summary>
        /// Vrátí true, pokud daný ukazatel ukazuje na místo uprostøed závorek
        /// nebo na místo uprostøed øetìzce
        /// </summary>
        /// <param name="e">Výraz</param>
        /// <param name="p">Zkoumaná pozice</param>
        public static bool IsInBracket(string e, int p) {
            int length = e.Length;

            int i = 0;
            int j = 0;

            while(i < length) {
                if((j = StringPosition(e, i)) > 0) {
                    if(p < j && p > i)
                        return true;
                    i = j;
                }
                else if((j = BracketPosition(e, i)) > 0) {
                    if(p < j && p > i)
                        return true;
                    i = j;
                }
                else
                    i++;
            }

            return false;
        }
    }
}
