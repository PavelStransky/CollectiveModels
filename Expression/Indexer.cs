using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// T��da pro vyhodnocen� indexov�n�
	/// </summary>
	public class Indexer: Atom {
		private object indexedItem;

		// Indexy
		private ArrayList indexes = new ArrayList();
		// N�zvy pomocn�ch prom�nn�ch, ur�uj�c�ch index posledn�ho prvku �ad
		private ArrayList endVariables = new ArrayList();

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="expression">V�raz funkce</param>
        /// <param name="parent">Rodi�</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
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
		/// Provede v�po�et funkce
		/// </summary>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        /// <returns>V�sledek v�po�tu</returns>
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
		/// Provede vyhodnocen� indexeru
		/// </summary>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        /// <param name="depth">Aktu�ln� hloubka</param>
		/// <param name="item">Prvek, kter� se vyhodnocuje</param>
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
				// [;] - cel� matice
				// [2;] - ��dek s indexem 2 jako vektor
				// [1...2;] - matice s vybran�mi ��dky
				// (tot� pro sloupce)
				// [1;3] - prvek s vybran�mi indexy

				// Vypo��t�me druh� index
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

				// Kv�li chyb�
				depth--;
			}

			return this.ManyIndexesError(depth - 1, this.indexes.Count);
		}

		/// <summary>
		/// Nastav� na kontext pomocnou prom�nnou s indexem posledn�ho prvku �ady
		/// </summary>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        /// <param name="item">Objekt, jeho� posledn� index nastavujeme</param>
		/// <param name="depth">Aktu�ln� hloubka</param>
		/// <param name="takeMatrixY">Item je matice a my budeme br�t jej� druh� rozm�r</param>
		/// <returns>Jm�no nastaven� prom�nn�, jinak null</returns>
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
		/// Vyma�e z kontextu koncovou prom�nnou
		/// </summary>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        private void ClearEndVariables(Context context) {
			foreach(string endVariable in this.endVariables)
				if(context.Contains(endVariable))
					context.Clear(endVariable);
		}

		/// <summary>
		/// Kontroluje, zda indexy maj� spr�vn� typ
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
		/// Chyba p�i �patn�m typu index�
		/// </summary>
		/// <param name="index">Objekt s indexy</param>
		/// <param name="depth">Hloubka</param>
		private object BadIndexTypeError(object index, int depth) {
			throw new ExpressionException(string.Format(errorMessageBadIndexType, depth, index.GetType().FullName),
				string.Format(errorMessageDetail, this.expression));
		}

		/// <summary>
		/// Chyba p�i �patn�m typu index�
		/// </summary>
		/// <param name="needed">Po�et pot�ebn�ch index�</param>
		/// <param name="count">Po�et zadan�ch index�</param>
		private object ManyIndexesError(int needed, int count) {
			throw new ExpressionException(string.Format(errorMessageManyIndexes, count, needed),
				string.Format(errorMessageDetail, this.expression));
		}

		/// <summary>
		/// Zjist�, zda se ve v�razu nach�z� znak pro d�lku �ady. Pokud ano, nahrad� jej n�zvem pomocn� prom�nn�
		/// </summary>
		/// <param name="e">V�raz</param>
		/// <param name="endVariable">Z�stupn� prom�nn� pro $</param>
		/// <returns>Nahrazen� v�raz</returns>
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
		/// Vr�t� nov� jedine�n� n�zev prom�nn�
		/// </summary>
		private static string GetEndVariable() {
			return string.Format(defaultEndVariable, countEndVariables++);
		}

		private const char endChar = '$';
		private const string defaultEndVariable = "$end{0}";

		private const string errorMessageManyIndexes = "Indexer m� p��li� mnoho index�. Zad�no: {0}, po�adov�no maxim�ln� {1}.";
		private const string errorMessageBadIndexType = "Index na pozici {0} m� �patn� typ '{1}'.";
		private const string errorMessageDetail = "V�raz: {0}";
	}
}
