using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Z�kladn� p�edek pro vzorec a funkci
	/// </summary>
	public abstract class Atom {
		/// <summary>
		/// Typy v�raz�
		/// </summary>
		protected enum ExpressionTypes {
			Transform,
			Formula, 
			Function, 
			Assignment, 
			Null, 
			Bool, 
			Int32, 
			Double, 
			String, 
			Variable, 
			ExpressionList,
			Indexer
		}

		// N� v�raz
		protected string expression;
		// Kontext
		protected Context context;
		// Atom o �rove� v�� (kv�li po��t�n� po�tu krok�)
		protected Atom parent;
        // Kv�li v�emo�n�m v�stup�m
        protected IOutputWriter writer;

		// Prom�nn�, kter� se vyskytuj� v ��sti v�razu
		protected ArrayList variables = new ArrayList();

        // Informace o prob�haj�c�m v�po�tu
        protected string infoText = string.Empty;

        /// <summary>
        /// Rodi�
        /// </summary>
        public Atom Parent { get { return this.parent; } }

        /// <summary>
        /// Informace o prob�haj�c�m v�po�tu
        /// </summary>
        public string InfoText { get { return this.infoText; } }

		/// <summary>
		/// N� v�raz
		/// </summary>
		public string Expression {get {return this.expression;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="context">Kontext</param>
		/// <param name="expression">V�raz funkce</param>
		/// <param name="parent">Rodi�</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
		public Atom(Context context, string expression, Atom parent, IOutputWriter writer) {
			this.Create(context, expression, parent, writer);
		}

		/// <summary>
		/// Vytvo�en� objektu
		/// </summary>
		private void Create(Context context, string expression, Atom parent, IOutputWriter writer) {
			this.context = context;
			this.parent = parent;
            this.writer = writer;
			this.expression = RemoveComment(expression);
			this.expression = RemoveNewLine(this.expression);
			this.expression = RemoveOutsideBracket(this.expression).Trim();
			CheckBrackets(this.expression);
			CheckSubstChar(this.expression);
		}

		/// <summary>
		/// Provede v�po�et ��sti v�razu
		/// </summary>
		/// <returns>V�sledek v�po�tu</returns>
		public virtual object Evaluate() {
			return null;
		}

		/// <summary>
		/// Statick� konstruktor
		/// </summary>
		static Atom() {
			unaryOperators = new Operators.Operators();
			unaryOperators.Add(new UnaryOperators.Plus());
			unaryOperators.Add(new UnaryOperators.Minus());

			binaryOperators = new Operators.Operators();
			binaryOperators.Add(new BinaryOperators.Plus());
			binaryOperators.Add(new BinaryOperators.Minus());
			binaryOperators.Add(new BinaryOperators.Times());
			binaryOperators.Add(new BinaryOperators.Divide());
			binaryOperators.Add(new BinaryOperators.Equals());
			binaryOperators.Add(new BinaryOperators.NotEquals());
			binaryOperators.Add(new BinaryOperators.GEquals());
			binaryOperators.Add(new BinaryOperators.LEquals());
			binaryOperators.Add(new BinaryOperators.Greater());
			binaryOperators.Add(new BinaryOperators.Lesser());
			binaryOperators.Add(new BinaryOperators.Interval());
			binaryOperators.Add(new BinaryOperators.ArrayAdd());
			binaryOperators.Add(new BinaryOperators.Power());
			binaryOperators.Add(new BinaryOperators.ArrayGen());

			functions = new Functions.FunctionDefinitions();
			functions.Add(new Functions.AddItem());
			functions.Add(new Functions.FnArray());
			functions.Add(new Functions.Array2Vector());
			functions.Add(new Functions.Clear());
			functions.Add(new Functions.CM());
			functions.Add(new Functions.DropColumns());
			functions.Add(new Functions.DropRows());
			functions.Add(new Functions.EigenSystem());
			functions.Add(new Functions.Eval());
			functions.Add(new Functions.Exit());
			functions.Add(new Functions.FnExport());
			functions.Add(new Functions.GenArray());
			functions.Add(new Functions.GetColumns());
			functions.Add(new Functions.GetRows());
			functions.Add(new Functions.LGraph());
			functions.Add(new Functions.DGraph());
			functions.Add(new Functions.FnImport());
			functions.Add(new Functions.Interval());
			functions.Add(new Functions.Join());
			functions.Add(new Functions.Length());
			functions.Add(new Functions.SLength());
			functions.Add(new Functions.Norm());
			functions.Add(new Functions.Normalize());
			functions.Add(new Functions.SNormalize());
			functions.Add(new Functions.Objects());
			functions.Add(new Functions.Permute());
			functions.Add(new Functions.Split());
			functions.Add(new Functions.SplitColumns());
			functions.Add(new Functions.SplitRows());
			functions.Add(new Functions.Start());
			functions.Add(new Functions.Stop());
			functions.Add(new Functions.String());
			functions.Add(new Functions.FnTimer());
			functions.Add(new Functions.Transpose());
			functions.Add(new Functions.FnVector());
            functions.Add(new Functions.FnMatrix());
			functions.Add(new Functions.Windows());
			functions.Add(new Functions.SwapDim());
			functions.Add(new Functions.FnFor());
			functions.Add(new Functions.FnIf());
			functions.Add(new Functions.FnHotKey());
			functions.Add(new Functions.FnGraphArray());
			functions.Add(new Functions.FnPoint());
			functions.Add(new Functions.Histogram());
			functions.Add(new Functions.CumulHistogram());
			functions.Add(new Functions.FnRegression());
			functions.Add(new Functions.FnPolynom());
			functions.Add(new Functions.FnPointVector());
			functions.Add(new Functions.Sort());
			functions.Add(new Functions.SortDesc());
			functions.Add(new Functions.Min());
			functions.Add(new Functions.Max());
			functions.Add(new Functions.MinAbs());
			functions.Add(new Functions.MaxAbs());
			functions.Add(new Functions.MinIndex());
			functions.Add(new Functions.MaxIndex());
			functions.Add(new Functions.MinAbsIndex());
			functions.Add(new Functions.MaxAbsIndex());
			functions.Add(new Functions.GetX());
			functions.Add(new Functions.GetY());
			functions.Add(new Functions.Spacing());
			functions.Add(new Functions.Mean());
			functions.Add(new Functions.Var());
			functions.Add(new Functions.Integrate());
			functions.Add(new Functions.Wigner());
			functions.Add(new Functions.FnType());

			functions.Add(new Functions.PC());
			functions.Add(new Functions.Symmetry());

			// Standardn� matematick� funkce
			functions.Add(new Functions.ArcTan());
            functions.Add(new Functions.Abs());
            functions.Add(new Functions.Log());
            functions.Add(new Functions.Sqrt());

			// Funkce vy�aduj�c� functions
			functions.Add(new Functions.FNames(functions));
			functions.Add(new Functions.Use(functions));
			functions.Add(new Functions.FnHelp(functions));
			functions.Add(new Functions.FullHelp(functions));

            // Funkce GCM
            functions.Add(new Functions.SimpleCGCM());
            functions.Add(new Functions.ExtendedCGCM1());
            functions.Add(new Functions.ExtendedCGCM2());
            functions.Add(new Functions.CGCMJ());
            functions.Add(new Functions.QGCM());
            functions.Add(new Functions.Equipotential());
            functions.Add(new Functions.TrajectoryM());
            functions.Add(new Functions.TrajectoryP());
            functions.Add(new Functions.InitialCondition());
            functions.Add(new Functions.Poincare());
            functions.Add(new Functions.FnSALI());
            functions.Add(new Functions.SALIZ());
            functions.Add(new Functions.SALIR());
            functions.Add(new Functions.SALIG());
            functions.Add(new Functions.EnergyLevels());
            functions.Add(new Functions.Energy());
            functions.Add(new Functions.Bounds());

            // Funkce IBM
            functions.Add(new Functions.CIBM());

            // Henon - Heiles
            functions.Add(new Functions.HH());
		}

		/// <summary>
		/// Odstran� koment��
		/// </summary>
		/// <param name="e">V�raz</param>
		private static string RemoveComment(string e) {
			int index = -1;
			while((index = e.IndexOf(commentMark, index + 1)) >= 0) {
				if(IsInString(e, index)) 
					continue;

				int newLineIndex = e.IndexOf('\n', index);
				if(newLineIndex < 0)
					e = e.Substring(0, index);
				else
					e = e.Substring(0, index) + e.Substring(newLineIndex, e.Length - newLineIndex);

				index--;
			}

			return e;
		}

		/// <summary>
		/// Odstran� znaky nov�ch ��dek a nahrad� je mezerami
		/// </summary>
		/// <param name="e">V�raz</param>
		private static string RemoveNewLine(string e) {
			return e.Replace("\r", string.Empty).Replace("\n", substNewLine);
		}

		/// <summary>
		/// Zkontroluje, zda se ve vzorci nevyskytuje z�stupn� znak (nesm� se vyskytovat)
		/// </summary>
		/// <param name="e">V�raz</param>
		private static void CheckSubstChar(string e) {
			int index = e.IndexOf(substitutionChar);
			if(index >= 0)
				throw new ExpressionException(string.Format(errorMessageSubstChar, substitutionChar),
					string.Format(errorMessageSubstCharDetail, e, index));
		}

		/// <summary>
		/// Zkontroluje, zda jsou spr�vn� zadan� z�vorky (oby�ejn� i indexov�)
		/// </summary>
		/// <param name="e">V�raz</param>
		private static void CheckBrackets(string e) {
			int numBrackets = 0;
			int i = 0;
			int openBracketIndex = -1;
			char closeBracketChar = closeIndexBracket;

			for(i = 0; i < e.Length; i++) {
				if(IsInString(e, i))
					continue;
				if(e[i] == openBracket && (numBrackets == 0 || closeBracketChar == closeBracket)) {
					numBrackets++;
					if(numBrackets == 1) {
						openBracketIndex = i;
						closeBracketChar = closeBracket;
					}
				}
				else if(e[i] == openIndexBracket && (numBrackets == 0 || closeBracketChar == closeIndexBracket)) {
					numBrackets++;
					if(numBrackets == 1) {
						openBracketIndex = i;
						closeBracketChar = closeIndexBracket;
					}
				}
				else if((e[i] == closeBracket || e[i] == closeIndexBracket) && numBrackets == 0)
					throw new ExpressionException(errorMessageBracketPosition, string.Format(errorMessageBracketPositionDetail, e, i));
				else if(e[i] == closeBracketChar) {
					numBrackets--;
					if(numBrackets == 0)
						CheckBrackets(e.Substring(openBracketIndex + 1, i - openBracketIndex - 1));
				}
			}

			if(numBrackets != 0) 
				throw new ExpressionException(errorMessageBracketNumber, string.Format(errorMessageBracketNumberDetail, e, numBrackets));
		}

		/// <summary>
		/// Odstran� v�echny vn�j�� z�vorky z v�razu
		/// </summary>
		/// <param name="e">V�raz</param>
		/// <returns>V�raz bez vn�j��ch z�vorek</returns>
		protected static string RemoveOutsideBracket(string e) {
			return RemoveOutsideBracket(openBracket, closeBracket, e);
		}

		/// <summary>
		/// Odstran� v�echny vn�j�� z�vorky z v�razu
		/// </summary>
		/// <param name="e">V�raz</param>
		/// <returns>V�raz bez vn�j��ch z�vorek</returns>
		protected static string RemoveOutsideIndexBracket(string e) {
			return RemoveOutsideBracket(openIndexBracket, closeIndexBracket, e);
		}

		/// <summary>
		/// Odstran� v�echny vn�j�� z�vorky z v�razu
		/// </summary>
		/// <param name="openBracket">Znak otev�rac� z�vorky</param>
		/// <param name="closeBracket">Znak zav�rac� z�vorky</param>
		/// <param name="e">V�raz</param>
		/// <returns>V�raz bez vn�j��ch z�vorek</returns>
		private static string RemoveOutsideBracket(char openBracket, char closeBracket, string e) {
			e = e.Trim();

			while(e.Length != 0) {
				if(e[0] == openBracket && e[e.Length - 1] == closeBracket) {
					string s = e.Substring(1, e.Length - 2).Trim();
					for(int i = 1; i < s.Length; i++) {
						string subs = s.Substring(0, i);
						if(BracketNumber(subs) < 0 || IndexBracketNumber(subs) < 0)
							return e;
					}
					e = s;
				}
				else
					return e;
			}

			return e;
		}

		/// <summary>
		/// True, pokud je pro zadan� v�raz BracketNumber != 0
		/// </summary>
		/// <param name="left">V�raz</param>
		public static bool IsInBracket(string left) {
			if(BracketNumber(left) != 0)
				return true;
			if(IndexBracketNumber(left) != 0)
				return true;

			return false;
		}

		/// <summary>
		/// True, pokud zadan� index je uzav�en v z�vorce
		/// </summary>
		/// <param name="e">V�raz</param>
		/// <param name="index">Index</param>
		public static bool IsInBracket(string e, int index) {
			return IsInBracket(e.Substring(0, index));
		}

		/// <summary>
		/// Vypo��t� rozd�l otev�rac� - uzav�rac� z�vorky
		/// </summary>
		/// <param name="e">V�raz</param>
		private static int BracketNumber(string e) {
			return BracketNumber(openBracket, closeBracket, e);
		}

		/// <summary>
		/// Vypo��t� rozd�l otev�rac� - uzav�rac� z�vorky indexeru
		/// </summary>
		/// <param name="e">V�raz</param>
		public static int IndexBracketNumber(string e) {
			return BracketNumber(openIndexBracket, closeIndexBracket, e);
		}

		/// <summary>
		/// Vypo��t� rozd�l otev�rac� - uzav�rac� z�vorky
		/// </summary>
		/// <param name="openBracket">Znak otev�rac� z�vorky</param>
		/// <param name="closeBracket">Znak zav�rac� z�vorky</param>
		/// <param name="e">V�raz</param>
		private static int BracketNumber(char openBracket, char closeBracket, string e) {
			int numOpen = 0;
			int index = -1;
			while((index = e.IndexOf(openBracket, index + 1)) >= 0) 
				if(!IsInString(e, index))
					numOpen++;

			int numClose = 0;
			index = -1;
			while((index = e.IndexOf(closeBracket, index + 1)) >= 0) 
				if(!IsInString(e, index))
					numClose++;
			
			return numOpen - numClose;
		}

		/// <summary>
		/// Zjist�, jak�ho typu je vstupn� ��st vzorce
		/// </summary>
		/// <param name="e">V�raz</param>
		protected static ExpressionTypes ExpressionType(string e) {
			if(e.Length == 0)
				return ExpressionTypes.Null;
			if(FindSeparatorPosition(e) > 0)
				return ExpressionTypes.ExpressionList;
			else if(FindAssignmentOperatorPosition(e) > 0)
				return ExpressionTypes.Assignment;
			else if(FindBinaryOperatorPosition(e) > 0)
				return ExpressionTypes.Formula;
			else if(FindUnaryOperatorPosition(e) == 0)
				return ExpressionTypes.Transform;
			else if(FindOpenIndexBracketPosition(e) > 0)
				return ExpressionTypes.Indexer;
			else if(FindOpenBracketPosition(e) > 0)
				return ExpressionTypes.Function;
			else if(numbers.IndexOf(e[0]) >= 0) {
				if(e.IndexOf('.') >= 0 || e.IndexOf(',') >= 0) 
					return ExpressionTypes.Double;
				else
					return ExpressionTypes.Int32;
			}
			else if(IsString(e))
				return ExpressionTypes.String;
			else if(IsBool(e))
				return ExpressionTypes.Bool;
			else
				return ExpressionTypes.Variable;
		}

		/// <summary>
		/// Najde polohu odd�lova�e
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		protected static int FindSeparatorPosition(string e) {
			int index = -1;
			while((index = e.IndexOf(separator, index + 1)) >= 0) {
				if(IsInString(e, index))
					continue;

				if(!IsInBracket(e, index)) 
					return index;
			}

			return -1;
		}

		/// <summary>
		/// Najde polohu oper�toru p�i�azen�
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		protected static int FindAssignmentOperatorPosition(string e) {
			int index = -1;
			while((index = e.IndexOf(assignmentOperator, index + 1)) >= 0) {
				if(IsInString(e, index))
					continue;

				if(!IsInBracket(e, index)) {
					// Kontrolujeme, zda se nejedn� o n�jak� z oper�tor� "<=, >=, =="
					string substring = e.Substring(System.Math.Max(index - 2, 0), 
						System.Math.Min(binaryOperators.MaxOperatorLength + 2, e.Length - index))
                        .Replace(openBracket.ToString(), string.Empty)
                        .Replace(closeBracket.ToString(), string.Empty);
					int pos = FindBinaryOperatorPosition(substring);
					// M��e b�t nap�. x =-12
					if(pos < 0 || substring.IndexOf(assignmentOperator, pos) < 0)
						return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// Najde polohu un�rn�ho oper�toru, kter� budeme zpracov�vat
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		protected static int FindUnaryOperatorPosition(string e) {
			string operatorName = string.Empty;
			return FindUnaryOperatorPosition(out operatorName, e);
		}

		/// <summary>
		/// Najde polohu un�rn�ho oper�toru, kter� budeme zpracov�vat
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <param name="operatorName">V�sledek - ozna�en� oper�toru</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		protected static int FindUnaryOperatorPosition(out string operatorName, string e) {
			return FindOperatorPosition(out operatorName, e, unaryOperators.OperatorPattern, false);
		}

		/// <summary>
		/// Najde polohu bin�rn�ho oper�toru, kter� budeme zpracov�vat
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		protected static int FindBinaryOperatorPosition(string e) {
			string operatorName = string.Empty;
			return FindBinaryOperatorPosition(out operatorName, e);
		}

		/// <summary>
		/// Najde polohu bin�rn�ho oper�toru, kter� budeme zpracov�vat
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <param name="operatorName">V�sledek - ozna�en� oper�toru</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		protected static int FindBinaryOperatorPosition(out string operatorName, string e) {
			return FindOperatorPosition(out operatorName, e, binaryOperators.OperatorPattern, true);
		}

		/// <summary>
		/// Najde polohu oper�toru, kter� budeme zpracov�vat
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <param name="operatorName">V�sledek - ozna�en� oper�toru</param>
		/// <param name="operatorPattern">Vzorec pro vyhled�v�n� pomoc� regul�rn�ch v�raz�</param>
		/// <param name="binary">True, pokud se jedn� o bin�rn� oper�tor, kter� se nesm� vyskytovat na prvn� pozici</param>
		/// <returns>Poloha oper�toru ve v�razu</returns>
		private static int FindOperatorPosition(out string operatorName, string e, string operatorPattern, bool binary) {
			MatchCollection matches = Regex.Matches(e, operatorPattern, RegexOptions.ExplicitCapture);

			foreach(Match match in matches) {
				if(match.Index == 0 && binary)
					continue;

				if(IsInString(e, match.Index))
					continue;

				if(!IsInBracket(e.Substring(0, match.Index))) {
					operatorName = match.Value;
					return match.Index;
				}
			}

			operatorName = string.Empty;
			return -1;
		}

		/// <summary>
		/// Najde polohu z�vorky s argumenty funkce
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha z�vorky ve v�razu</returns>
		protected static int FindOpenBracketPosition(string e) {
			return FindOpenBracketPosition(openBracket, e);
		}

		/// <summary>
		/// Najde polohu z�vorky indexeru
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha z�vorky ve v�razu</returns>
		protected static int FindOpenIndexBracketPosition(string e) {
			return FindOpenBracketPosition(openIndexBracket, e);
		}

		/// <summary>
		/// Najde polohu z�vorky
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha z�vorky ve v�razu</returns>
		protected static int FindOpenBracketPosition(char openBracket, string e) {
			int index = -1;
			while((index = e.IndexOf(openBracket, index + 1)) >= 0)
				if(!IsInString(e, index) && !IsInBracket(e, index))
					return index;

			return -1;
		}

		/// <summary>
		/// Najde polohu POSLEDN� otev�rac� z�vorky indexeru
		/// </summary>
		/// <param name="e">V�raz, ve kter�m hled�</param>
		/// <returns>Poloha z�vorky ve v�razu</returns>
		protected static int FindLastOpenIndexBracketPosition(string e) {
			int result = e.Length - 1;

			while((result = e.LastIndexOf(openIndexBracket)) >= 0) {
				if(!IsInBracket(e, result))
					return result;
			}

			return -1;
		}

		/// <summary>
		/// Rozhodne, zda v�raz je samostatn� �et�zec
		/// </summary>
		/// <param name="e">V�raz</param>
		protected static bool IsString(string e) {
			if(e.Length < 2 || e[0] != quotationMark || e[e.Length - 1] != quotationMark)
				return false;

			int index = 0;
			while((index = e.IndexOf(quotationMark, index + 1)) != -1) {
				if(e[index - 1] != specialCharMark && !(index == e.Length - 1 && e[index - 1] != specialCharMark))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Rozhodne, zda v�raz je logick� prom�nn� (true, false)
		/// </summary>
		/// <param name="e">V�raz</param>
		protected static bool IsBool(string e) {
			if(e == boolTrue || e == boolFalse)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Vr�t� true, pokud se uveden� pozice nach�z� uprost�ed �et�zce
		/// </summary>
		/// <param name="e">Vstupn� v�raz</param>
		/// <param name="position">Pozice</param>
		protected static bool IsInString(string e, int position) {
			bool result = false;

			for(int i = 0; i <= position; i++)
				if(e[i] == quotationMark && (i == 0 || e[i - 1] != specialCharMark))
					result = !result;

			return result;
		}

		/// <summary>
		/// Rozhodne, jak� objekt z ��sti v�razu vytvo��
		/// </summary>
		/// <param name="expression">V�raz funkce</param>
		/// <returns>Vytvo�en� objekt</returns>
		protected object CreateAtomObject(string expression) {
			object retValue = null;

			switch(ExpressionType(expression)) {
				case ExpressionTypes.Transform:
					retValue = new Transform(this.context, expression, this, this.writer);
					break;
				case ExpressionTypes.Formula:
					retValue = new Formula(this.context, expression, this, this.writer);
					break;
				case ExpressionTypes.Function:
					retValue = new Function(this.context, expression, this, this.writer);
					break;
				case ExpressionTypes.Assignment:
					retValue = new Assignment(this.context, expression, this, this.writer);
					break;
				case ExpressionTypes.ExpressionList:
					retValue = new ExpressionList(this.context, expression, this, this.writer);
					break;
				case ExpressionTypes.Indexer:
					retValue = new Indexer(this.context, expression, this, this.writer);
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

			// Do sv�ho seznamu prom�nn�ch p�id�me v�echny prom�nn� z ni��� ��sti v�razu
			if(retValue is Atom)
				this.AddVariablesToList((retValue as Atom).variables);

			return retValue;
		}

		/// <summary>
		/// Zjist�, zda v�raz nen� u�ivatelem definovan� �et�zec (je v uvozovk�ch)
		/// </summary>
		/// <param name="e">V�raz</param>
		/// <returns>Je - li �etezec, pak �et�zec, jinak null</returns>
		protected static string ParseString(string e) {
			if(IsString(e))
				return e.Substring(1, e.Length - 2).Replace(specialCharMark.ToString() + quotationMark.ToString(), quotationMark.ToString());
			else
				return null;
		}

		/// <summary>
		/// P�evede logickou prom�nnou zadanou v textov�m �et�zci na typ bool
		/// </summary>
		/// <param name="e">V�raz</param>
		protected static bool ParseBool(string e) {
			if(e == boolTrue)
				return true;
			else if(e == boolFalse)
				return false;
			else
				throw new ExpressionException(errorMessageNotBool, string.Format(errorMessageNotBoolDetail, e));
		}

		/// <summary>
		/// P�id� n�zvy prom�nn�ch do vlastn�ho seznamu prom�nn�ch
		/// </summary>
		/// <param name="variable">N�zvy prom�nn�ch vnit�n� ��sti v�razu</param>
		/// <returns>Po�et p�idan�ch prom�nn�ch</returns>
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
		/// P�id� n�zev prom�nn� do seznamu prom�nn�ch
		/// </summary>
		/// <param name="variable">N�zev prom�nn�</param>
		/// <returns>Aktu�ln� index</returns>
		protected int AddVariableToList(string variable) {
			if(!this.variables.Contains(variable))
				return this.variables.Add(variable);
			else
				return this.variables.IndexOf(variable);
		}

		/// <summary>
		/// Provede v�po�et dan� ��sti v�razu
		/// </summary>
		/// <param name="context">Kontext</param>
		/// <param name="expression">V�raz funkce</param>
		/// <param name="parent">Rodi�</param>
		/// <returns>Objekt z�skan� v�po�tem</returns>
		public static object EvaluateAtomObject(Context context, object operand) {
			object retValue;

			if(operand is Atom) {
				retValue = (operand as Atom).Evaluate();
			}
			else if(operand is string) {
				string s = ParseString(operand as String);
				if(s == null)
					retValue = context[operand as string];
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
		/// Odd�l� od sebe jednotliv� parametry v z�vorce
		/// </summary>
		/// <param name="e">Vstupn� v�raz</param>
		protected static string [] SplitArguments(string e) {
			StringBuilder s = new StringBuilder(e.Length);

			for(int i = 0; i < e.Length; i++) {
				if(e[i] == separator && !IsInBracket(e, i) && !IsInString(e, i))
					s.Append(substitutionChar);
				else
					s.Append(e[i]);
			}

			return s.ToString().Split(substitutionChar);
		}

		protected const string numbers = "0123456789 ";
		protected const char openBracket = '(';
		protected const char closeBracket = ')';
		protected const char openIndexBracket = '[';
		protected const char closeIndexBracket = ']';
		protected const char assignmentOperator = '=';
		protected const char separator = ';';
		protected const char semicolon = ';';
		protected const char quotationMark = '"';
		protected const char specialCharMark = '\\';
		protected const char substitutionChar = '`';

		private const string substNewLine = "   ";
		private const string boolTrue = "true";
		private const string boolFalse = "false";

		protected const string commentMark = "%%";

		protected static readonly Operators.Operators unaryOperators;
		protected static readonly Operators.Operators binaryOperators;
		protected static readonly Functions.FunctionDefinitions functions;

		private const string errorMessageBracketNumber = "Ve v�razu je chybn� po�et z�vorek.";
		private const string errorMessageBracketNumberDetail = "V�raz: {0}\nRozd�l otev�rac� - uzav�rac� z�vorky: {1}";

		private const string errorMessageBracketPosition = "Z�vorky ve v�razu jsou �patn� uspo��d�ny.";
		private const string errorMessageBracketPositionDetail = "V�raz: {0}\nPozice: {1}";

		private const string errorMessageNotBool = "V�raz nelze vyhodnotit jako logickou prom�nnou.";
		private const string errorMessageNotBoolDetail = "V�raz: {0}";

		private const string errorMessageSubstChar = "Ve v�razu se nesm� vyskytovat znak '{0}'";
		private const string errorMessageSubstCharDetail = "V�raz: {0}\nPozice: {1}";
	}
}
