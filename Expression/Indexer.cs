using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// Tøída pro vyhodnocení indexování
	/// </summary>
	public class Indexer: Atom {
		private object indexedItem;

		// Indexy
		private ArrayList indexes = new ArrayList();
		// Názvy pomocných promìnných, urèujících index posledního prvku øad
		private ArrayList endVariables = new ArrayList();

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">Výraz funkce</param>
        /// <param name="parent">Rodiè</param>
        /// <param name="writer">Writer pro textové výstupy</param>
        public Indexer(string expression, Atom parent, IOutputWriter writer)
            : base(expression, parent, writer) {
            int pos = FindLastOpenIndexBracketPosition(this.expression);

            this.indexedItem = this.CreateAtomObject(RemoveOutsideBracket(this.expression.Substring(0, pos)).Trim().ToLower());
            string inds = this.expression.Substring(pos + 1, this.expression.Length - pos - 2).Trim();

            if(inds.Length == 0)
                return;

            string[] a = SplitArguments(inds);
            for(int i = 0; i < a.Length; i++) {
                string arg = RemoveOutsideBracket(a[i]).Trim();
                string endVariable = GetEndVariable();
                this.endVariables.Add(endVariable);
                arg = ReplaceEndVariable(arg, endVariable);
                this.indexes.Add(this.CreateAtomObject(arg));
            }
        }

		/// <summary>
		/// Provede výpoèet funkce
		/// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        /// <returns>Výsledek výpoètu</returns>
		public override object Evaluate(Context context) {
            object result = null;			
			object indexed = EvaluateAtomObject(context, indexedItem);

			try {
				result = this.Evaluate(context, 0, indexed);
			}
			catch(Exception e) {
				throw e;
			}
			finally {
				this.ClearEndVariables(context);
			}

			return result;
		}
		
		/// <summary>
		/// Provede vyhodnocení indexeru
		/// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        /// <param name="depth">Aktuální hloubka</param>
		/// <param name="item">Prvek, který se vyhodnocuje</param>
		private object Evaluate(Context context, int depth, object item) {
			if(depth >= this.indexes.Count)
				return item;

			this.SetEndVariable(context, item, depth, false);
			object ind = EvaluateAtomObject(context, this.indexes[depth]);
			this.CheckIndexType(ind, depth);

			depth++;
			if(item is TArray) {
				TArray array = item as TArray;

				if(ind == null) {
					TArray result = new TArray();
					for(int i = 0; i < array.Count; i++)
						result.Add(this.Evaluate(context, depth, array[i]));
					return result;
				}
				else if(ind is int)
					return this.Evaluate(context, depth, array[(int)ind]);
				else if(ind is TArray) {
					TArray iArray = ind as TArray;
					TArray result = new TArray();
					for(int i = 0; i < iArray.Count; i++)
						result.Add(this.Evaluate(context, depth, array[(int)iArray[i]]));
					return result;
				}
			}
			else if(item is Vector) {
				if(depth < this.indexes.Count)
					this.ManyIndexesError(depth, this.indexes.Count);

				Vector vector = item as Vector;

				if(ind == null) 
					return item;
				else if(ind is int)
					return vector[(int)ind];
				else if(ind is TArray) {
					TArray iArray = ind as TArray;
					Vector result = new Vector(iArray.Count);
					for(int i = 0; i < iArray.Count; i++)
						result[i] = vector[(int)iArray[i]];
					return result;
				}
			}
			else if(item is PointVector) {
				if(depth < this.indexes.Count)
					this.ManyIndexesError(depth, this.indexes.Count);

				PointVector pv = item as PointVector;

				if(ind == null) 
					return item;
				else if(ind is int)
					return pv[(int)ind];
				else if(ind is TArray) {
					TArray iArray = ind as TArray;
					PointVector result = new PointVector(iArray.Count);
					for(int i = 0; i < iArray.Count; i++)
						result[i] = pv[(int)iArray[i]];
					return result;
				}
			}
			else if(item is Matrix) {
				// Tady pozor. 
				// [;] - celá matice
				// [2;] - øádek s indexem 2 jako vektor
				// [1...2;] - matice s vybranými øádky
				// (totéž pro sloupce)
				// [1;3] - prvek s vybranými indexy

				// Vypoèítáme druhý index
				object indy = null;
				if(depth < this.indexes.Count) {
					this.SetEndVariable(context, item, depth, true);
					indy = EvaluateAtomObject(context, this.indexes[depth]);
					this.CheckIndexType(ind, depth);
				}

				depth++;
				if(depth < this.indexes.Count)
					this.ManyIndexesError(depth, this.indexes.Count);

				Matrix matrix = item as Matrix;

				if(ind == null && indy == null)
					return matrix;
				else if(ind is int && indy == null) 
					return matrix.GetRowVector((int)ind);
				else if(ind == null && indy is int)
					return matrix.GetColumnVector((int)indy);
				else if(ind is int && indy is int)
					return matrix[(int)ind, (int)indy];
				else if(ind is TArray) {
					TArray xArray = ind as TArray;

					if(indy == null) {
						Matrix result = new Matrix(xArray.Count, matrix.LengthY);
						for(int i = 0; i < xArray.Count; i++)
							for(int j = 0; j < matrix.LengthY; j++)
								result[i, j] = matrix[(int)xArray[i], j];
						return result;
					}
					else if(indy is TArray) {
						TArray yArray = indy as TArray;

						Matrix result = new Matrix(xArray.Count, yArray.Count);
						for(int i = 0; i < xArray.Count; i++)
							for(int j = 0; j < yArray.Count; j++)
								result[i, j] = matrix[(int)xArray[i], (int)yArray[j]];
						return result;
					}
					else if(indy is int) {
						int y = (int)indy;

						Vector result = new Vector(xArray.Count);
						for(int i = 0; i < xArray.Count; i++)
							result[i] = matrix[(int)xArray[i], y];
						return result;
					}
					else
						return this.BadIndexTypeError(indy, depth - 1);
				}
				else if(indy is TArray) {
					TArray yArray = indy as TArray;

					if(ind == null) {
						Matrix result = new Matrix(matrix.LengthX, yArray.Count);
						for(int i = 0; i < matrix.LengthX; i++)
							for(int j = 0; j < yArray.Count; j++)
								result[i, j] = matrix[i, (int)yArray[j]];
						return result;
					}
					else if(ind is int) {
						int x = (int)ind;

						Vector result = new Vector(yArray.Count);
						for(int j = 0; j < yArray.Count; j++)
							result[j] = matrix[x, (int)yArray[j]];
						return result;
					}
				}

				// Kvùli chybì
				depth--;
			}

			return this.ManyIndexesError(depth - 1, this.indexes.Count);
		}

		/// <summary>
		/// Nastaví na kontext pomocnou promìnnou s indexem posledního prvku øady
		/// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        /// <param name="item">Objekt, jehož poslední index nastavujeme</param>
		/// <param name="depth">Aktuální hloubka</param>
		/// <param name="takeMatrixY">Item je matice a my budeme brát její druhý rozmìr</param>
		/// <returns>Jméno nastavené promìnné, jinak null</returns>
		private string SetEndVariable(Context context, object item, int depth, bool takeMatrixY) {
			if(this.endVariables.Count < depth || this.endVariables[depth] == null)
				return null;

			string endVariable = this.endVariables[depth] as string;
			int end = 0;

			if(takeMatrixY)
				end = (item as Matrix).LengthY - 1;
			else if(item is TArray)
				end = (item as TArray).Count - 1;
			else if(item is Vector)
				end = (item as Vector).Length - 1;
			else if(item is Matrix)
				end = (item as Matrix).LengthX - 1;

			context.SetVariable(endVariable, end);
			return endVariable;	
		}

		/// <summary>
		/// Vymaže z kontextu koncovou promìnnou
		/// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        private void ClearEndVariables(Context context) {
			foreach(string endVariable in this.endVariables)
				if(context.Contains(endVariable))
					context.Clear(endVariable);
		}

		/// <summary>
		/// Kontroluje, zda indexy mají správný typ
		/// </summary>
		/// <param name="index">Objekt s indexy</param>
		/// <param name="depth">Hloubka</param>
		private object CheckIndexType(object index, int depth) {
			if(index != null && !(index is TArray) && !(index is int))
				return this.BadIndexTypeError(index, depth);
			if(index is TArray && (index as TArray).ItemTypeName != typeof(int).FullName)
				return this.BadIndexTypeError((index as TArray)[0], depth);

			return null;
		}

		/// <summary>
		/// Chyba pøi špatném typu indexù
		/// </summary>
		/// <param name="index">Objekt s indexy</param>
		/// <param name="depth">Hloubka</param>
		private object BadIndexTypeError(object index, int depth) {
			throw new ExpressionException(string.Format(errorMessageBadIndexType, depth, index.GetType().FullName),
				string.Format(errorMessageDetail, this.expression));
		}

		/// <summary>
		/// Chyba pøi špatném typu indexù
		/// </summary>
		/// <param name="needed">Poèet potøebných indexù</param>
		/// <param name="count">Poèet zadaných indexù</param>
		private object ManyIndexesError(int needed, int count) {
			throw new ExpressionException(string.Format(errorMessageManyIndexes, count, needed),
				string.Format(errorMessageDetail, this.expression));
		}

		/// <summary>
		/// Zjistí, zda se ve výrazu nachází znak pro délku øady. Pokud ano, nahradí jej názvem pomocné promìnné
		/// </summary>
		/// <param name="e">Výraz</param>
		/// <param name="endVariable">Zástupná promìnná pro $</param>
		/// <returns>Nahrazený výraz</returns>
		private static string ReplaceEndVariable(string e, string endVariable) {
			int index = -1;

			while((index = (e.IndexOf(endChar, index + 1))) >= 0) {
				if(!IsInString(e, index)) 
					e = string.Format("{0}{1}{2}", e.Substring(0, index), endVariable, e.Substring(index + 1, e.Length - index - 1));
			}

			return e;
		}

		private static int countEndVariables = 0;

		/// <summary>
		/// Vrátí nový jedineèný název promìnné
		/// </summary>
		private static string GetEndVariable() {
			return string.Format(defaultEndVariable, countEndVariables++);
		}

		private const char endChar = '$';
		private const string defaultEndVariable = "$end{0}";

		private const string errorMessageManyIndexes = "Indexer má pøíliš mnoho indexù. Zadáno: {0}, požadováno maximálnì {1}.";
		private const string errorMessageBadIndexType = "Index na pozici {0} má špatný typ '{1}'.";
		private const string errorMessageDetail = "Výraz: {0}";
	}
}
