using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// T��da pro vyhodnocen� indexov�n�
	/// </summary>
	public class Indexer: Atom {
        /// <summary>
        /// T��da zapouz�uj�c� indexy spolu s indik�torem Shrinku
        /// </summary>
        private class Indexes {
            private int[][] index;
            private bool[] shrink;
            private int rank;

            /// <summary>
            /// Indexy
            /// </summary>
            public int[][] Index { get { return this.index; } }

            /// <summary>
            /// True, pokud se v dan�m rozm�ru d�l� shrink
            /// </summary>
            public bool[] Shrink { get { return this.shrink; } }

            /// <summary>
            /// D�lka
            /// </summary>
            public int Rank { get { return this.rank; } }

            /// <summary>
            /// Kontruktor
            /// </summary>
            /// <param name="rank">Po�et �ad index�</param>
            public Indexes(int rank) {
                this.rank = rank;

                this.index = new int[rank][];
                this.shrink = new bool[rank];
            }

            /// <summary>
            /// Nastav� jeden rank
            /// </summary>
            /// <param name="r">Rank</param>
            /// <param name="index">Indexy ranku</param>
            /// <param name="shrink">Bude se d�lat shrink?</param>
            public void Set(int r, int[] index, bool shrink) {
                this.index[r] = index;
                this.shrink[r] = shrink;
            }
        }

        // Objekt, kter� indexujeme
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
        public Indexer(string expression, Atom parent)
            : base(expression, parent) {
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

        #region P�i�azen�
        // P�i�azovac� funkce z p�edchoz� �rovn�
        private Assignment.AssignmentFunction assignFn;
        private Guider guider;

        private object AssignFn(object o) {
            try {
                Indexes index = this.EvaluateIndexes(this.guider, o);

                // Indexace
                if(o is TArray) {
                    if(index.Rank > (o as TArray).Rank)
                        this.ManyIndexesError((o as TArray).Rank, index.Rank);
                    (o as TArray).SetValue(index.Index, assignFn);
                }

                else if(o is Vector) {
                    if(index.Rank != 1)
                        this.ManyIndexesError(1, index.Rank);

                    int[] i = index.Index[0];
                    int l = i.Length;
                    Vector v = o as Vector;

                    for(int j = 0; j < l; j++) {
                        object result = this.assignFn(v[i[j]]);
                        v[i[j]] = (result is double) ? (double)result : (double)(int)result;
                    }
                }
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                this.ClearEndVariables(this.guider.Context);
            }

            return o;
        }

        /// <summary>
        /// Na zadan� m�sta indexeru p�i�ad� danou hodnotu
        /// </summary>
        /// <param name="guider">Pr�vodce v�po�tu</param>
        /// <param name="value">Hodnota</param>
        public void Evaluate(Guider guider, Assignment.AssignmentFunction assignFn) {
            this.assignFn = assignFn;
            this.guider = guider;

            if(indexedItem is Indexer) {
                (indexedItem as Indexer).Evaluate(guider, this.AssignFn);
            }
            else {
                this.AssignFn(EvaluateAtomObject(guider, this.indexedItem));
            }
        }
        #endregion

        /// <summary>
		/// Provede v�po�et funkce
		/// </summary>
        /// <param name="guider">Pr�vodce v�po�tu</param>
        /// <returns>V�sledek v�po�tu</returns>
		public override object Evaluate(Guider guider) {
            object result = null;			

			try {
                object o = EvaluateAtomObject(guider, indexedItem);

                // Nejprve vypo��t�me v�echny indexy
                Indexes index = this.EvaluateIndexes(guider, o);

                // Nyn� indexace
                if(o is Vector) {
                    if(index.Rank > 1)
                        this.ManyIndexesError(1, index.Rank);

                    if(index.Shrink[0])
                        result = (o as Vector)[index.Index[0][0]];
                    else
                        result = (o as Vector)[index.Index[0]];
                }

                else if(o is PointVector) {
                    if(index.Rank > 1)
                        this.ManyIndexesError(1, index.Rank);
//                    result = (o as PointVector)[index[0]];
                }
                else if(o is TArray) {
                    if(index.Rank > (o as TArray).Rank)
                        this.ManyIndexesError((o as TArray).Rank, index.Rank);
                    
                    result = (o as TArray).GetSubArray(index.Index, index.Shrink);
                }
			}
			catch(Exception e) {
				throw e;
			}
			finally {
				this.ClearEndVariables(guider.Context);
			}

			return result;
		}

        /// <summary>
        /// Provede v�po�et v�ech index�
        /// </summary>
        /// <param name="guider">Pr�vodce v�po�tem</param>
        /// <param name="o">Indexovan� object</param>
        private Indexes EvaluateIndexes(Guider guider, object o) {
            // Nejprve vypo��t�me v�echny indexy
            int count = this.indexes.Count;
            Indexes index = new Indexes(count);

            for(int i = 0; i < count; i++) {
                this.SetEndVariable(guider.Context, o, i);
                object ind = EvaluateAtomObject(guider, this.indexes[i]);

                if(ind == null) {
                    int length = this.GetLength(o, i);
                    int[] ji = new int[length];
                    for(int j = 0; j < length; j++)
                        ji[j] = j;

                    index.Set(i, ji, false);
                }
                else
                    this.ParseIndexes(ind, i, index);
            }

            return index;
        }

        /// <summary>
        /// Vr�t� d�lku objektu v zadan� hloubce
        /// </summary>
        /// <param name="item">Objekt</param>
        /// <param name="depth">Hloubka</param>
        private int GetLength(object item, int depth) {
            int length = 0;

            if(item is TArray)
                length = (item as TArray).GetLength(depth);
            else if(item is Vector)
                length = (item as Vector).Length;
            else if(item is Matrix) {
                if(depth == 0)
                    length = (item as Matrix).LengthX;
                else
                    length = (item as Matrix).LengthY;
            }
            else if(item is PointVector)
                length = (item as PointVector).Length;

            return length;
        }

		/// <summary>
		/// Nastav� na kontext pomocnou prom�nnou s indexem posledn�ho prvku �ady
		/// </summary>
        /// <param name="context">Kontext, na kter�m se spou�t� v�po�et</param>
        /// <param name="item">Objekt, jeho� posledn� index nastavujeme</param>
		/// <param name="depth">Hloubka</param>
		/// <returns>Jm�no nastaven� prom�nn�, jinak null</returns>
		private void SetEndVariable(Context context, object item, int depth) {
			string endVariable = this.endVariables[depth] as string;
			context.SetVariable(endVariable, this.GetLength(item, depth) - 1);
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
		/// <param name="indexIn">Objekt s indexy</param>
		/// <param name="depth">Hloubka</param>
        /// <param name="indexOut">V�stup index�</param>
		private void ParseIndexes(object indexIn, int depth, Indexes indexOut) {            
            if(indexIn is int) {
                int[] i = new int[1];
                i[0] = (int)indexIn;
                indexOut.Set(depth, i, true);

                return;
            }

            Type type = indexIn.GetType();
            if(indexIn is TArray) {
                TArray ta = indexIn as TArray;
                type = ta.GetItemType();

                if(type == typeof(int)) {
                    if(!ta.Is1D)
                        throw new ExpressionException(string.Format(errorMessageBadIndexRank, ta.Rank),
                            string.Format(errorMessageDetail, this.expression));

                    int length = ta.GetNumElements();
                    int[] i = new int[length];
                    
                    int k = 0;
                    ta.ResetEnumerator();
                    foreach(int ti in ta) {
                        i[k++] = ti;
                    }

                    indexOut.Set(depth, i, false);

                    return;
                }
            }

            throw new ExpressionException(string.Format(errorMessageBadIndexType, depth, type.FullName),
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

        private const string errorMessageBadIndexRank = "The indexing Array has {0} dimensions. Only 1 is required.";
		private const string errorMessageManyIndexes = "Indexer m� p��li� mnoho index�. Zad�no: {0}, po�adov�no maxim�ln� {1}.";
		private const string errorMessageBadIndexType = "Index na pozici {0} m� �patn� typ '{1}'.";
		private const string errorMessageDetail = "V�raz: {0}";
	}
}
