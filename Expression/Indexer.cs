using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression {
	/// <summary>
	/// Tøída pro vyhodnocení indexování
	/// </summary>
	public class Indexer: Atom {
        /// <summary>
        /// Tøída zapouzøující indexy spolu s indikátorem Shrinku
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
            /// True, pokud se v daném rozmìru dìlá shrink
            /// </summary>
            public bool[] Shrink { get { return this.shrink; } }

            /// <summary>
            /// Délka
            /// </summary>
            public int Rank { get { return this.rank; } }

            /// <summary>
            /// Kontruktor
            /// </summary>
            /// <param name="rank">Poèet øad indexù</param>
            public Indexes(int rank) {
                this.rank = rank;

                this.index = new int[rank][];
                this.shrink = new bool[rank];
            }

            /// <summary>
            /// Nastaví jeden rank
            /// </summary>
            /// <param name="r">Rank</param>
            /// <param name="index">Indexy ranku</param>
            /// <param name="shrink">Bude se dìlat shrink?</param>
            public void Set(int r, int[] index, bool shrink) {
                this.index[r] = index;
                this.shrink[r] = shrink;
            }
        }

        // Objekt, který indexujeme
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
        public Indexer(string expression, Atom parent)
            : base(expression, parent) {
            ArrayList parts = GetIndexes(this.expression);

            this.indexedItem = this.CreateAtomObject(parts[0] as string);
            this.endVariables = parts[2] as ArrayList;

            foreach(string s in (parts[1] as ArrayList)) 
                this.indexes.Add(this.CreateAtomObject(s));
        }

        #region Assignment
        // Pøiøazovací funkce z pøedchozí úrovnì
        private Atom.AssignmentFunction assignFn;
        private Guider guider;

        /// <summary>
        /// Assignment function
        /// </summary>
        private object AssignFn(object o) {
            try {
                Indexes index = this.EvaluateIndexes(this.guider, o);

                // Indexace
                if(o is TArray) {
                    if(index.Rank > (o as TArray).Rank)
                        this.ManyIndexesError((o as TArray).Rank, index.Rank);
                    (o as TArray).SetValue(index.Index, this.assignFn);
                }

                else if(o is List) {
                    if(index.Rank != 1)
                        this.ManyIndexesError(1, index.Rank);

                    int[] i = index.Index[0];
                    int c = i.Length;

                    List l = o as List;
                    
                    for(int j = 0; j < c; j++) 
                        (l as ArrayList)[i[j]] = this.AssignFn(l[i[j]]);
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

                else if(o is PointVector) {
                    if(index.Rank != 1)
                        this.ManyIndexesError(1, index.Rank);

                    int[] i = index.Index[0];
                    int l = i.Length;
                    PointVector pv = o as PointVector;

                    for(int j = 0; j < l; j++){
                        object result = this.assignFn(pv[i[j]]);
                        pv[i[j]] = (PointD)result;
                    }
                }

                else if(o is Matrix) {
                    if(index.Rank != 2)
                        this.ManyIndexesError(2, index.Rank);

                    int[] i = index.Index[0];
                    int[] j = index.Index[1];
                    int il = i.Length;
                    int jl = j.Length;
                    Matrix m = o as Matrix;

                    for(int k = 0; k < il; k++)
                        for(int l = 0; l < jl; l++) {
                            object result = this.assignFn(m[i[k], j[l]]);
                            if(result is int)
                                m[i[k], j[l]] = (double)(int)result;
                            else
                                m[i[k], j[l]] = (double)result;
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
        /// Na zadaná místa indexeru pøiøadí danou hodnotu
        /// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <param name="value">Hodnota</param>
        public void Evaluate(Guider guider, Atom.AssignmentFunction assignFn) {
            this.assignFn = assignFn;
            this.guider = guider;

            if(this.indexedItem is Indexer) {
                (this.indexedItem as Indexer).Evaluate(guider, this.AssignFn);
            }
            else {
                this.AssignFn(EvaluateAtomObject(guider, this.indexedItem));
            }
        }
        #endregion

        #region Getting
        /// <summary>
        /// Delegát funkce, která provede výbìr prvku
        /// </summary>
        public delegate object GetFunction(object o);
        private GetFunction getFn;

        /// <summary>
        /// Getting function
        /// </summary>
        private object GetFn(object o) {
            object result = null;

            try {
                // Nejprve vypoèítáme všechny indexy
                Indexes index = this.EvaluateIndexes(this.guider, o);

                // Nyní indexace
                if(o is Vector) {
                    if(index.Rank > 1)
                        this.ManyIndexesError(1, index.Rank);

                    if(index.Shrink[0])
                        result = (o as Vector)[index.Index[0][0]];
                    else {
                        int[] ind = index.Index[0];
                        int l = ind.Length;
                        Vector v = new Vector(l);
                        for(int i = 0; i < l; i++)
                            v[i] = (o as Vector)[ind[i]];
                        result = v;
                    }
                }

                else if(o is PointVector) {
                    if(index.Rank > 1)
                        this.ManyIndexesError(1, index.Rank);

                    if(index.Shrink[0])
                        result = (o as PointVector)[index.Index[0][0]];
                    else {
                        int[] ind = index.Index[0];
                        int l = ind.Length;
                        PointVector pv = new PointVector(l);

                        for(int i = 0; i < l; i++)
                            pv[i] = (o as PointVector)[ind[i]];

                        result = pv;
                    }
                }

                else if(o is Matrix) {
                    if(index.Rank > 2)
                        this.ManyIndexesError(2, index.Rank);

                    // Only row index
                    if(index.Rank == 1) {
                        if(index.Shrink[0])
                            return (o as Matrix).GetRowVector(index.Index[0][0]);
                        else {
                            int[] ind = index.Index[0];
                            int lx = ind.Length;
                            int ly = (o as Matrix).LengthY;
                            Matrix m = new Matrix(lx, ly);
                            for(int i = 0; i < lx; i++)
                                m.SetRowVector(i, (o as Matrix).GetRowVector(ind[i]));
                        }
                    }
                    // Both indexes
                    else {
                        if(index.Shrink[0] && index.Shrink[1])
                            result = (o as Matrix)[index.Index[0][0], index.Index[1][0]];
                        else if(index.Shrink[0]) {
                            int indx1 = index.Index[0][0];
                            int[] indy = index.Index[1];
                            int l = indy.Length;

                            Vector v = new Vector(l);
                            for(int i = 0; i < l; i++)
                                v[i] = (o as Matrix)[indx1, indy[i]];

                            result = v;
                        }
                        else if(index.Shrink[1]) {
                            int[] indx = index.Index[0];
                            int indy1 = index.Index[1][0];
                            int l = indx.Length;

                            Vector v = new Vector(l);
                            for(int i = 0; i < l; i++)
                                v[i] = (o as Matrix)[indx[i], indy1];

                            result = v;
                        }
                        else {
                            int[] indx = index.Index[0];
                            int[] indy = index.Index[1];
                            int lx = indx.Length;
                            int ly = indy.Length;

                            Matrix m = new Matrix(lx, ly);
                            for(int i = 0; i < lx; i++)
                                for(int j = 0; j < ly; j++)
                                    m[i, j] = (o as Matrix)[indx[i], indy[j]];

                            result = m;
                        }
                    }
                }

                else if(o is List) {
                    if(index.Rank > 1)
                        this.ManyIndexesError(1, index.Rank);

                    if(index.Rank == 0) {
                        index = new Indexes(1);
                        int c = (o as List).Count;
                        int[] ind = new int[c];
                        for(int i = 0; i < c; i++)
                            ind[i] = i;
                        index.Set(0, ind, false);
                    }

                    if(index.Shrink[0])
                        result = this.getFn((o as List)[index.Index[0][0]]);
                    else {
                        int l = index.Index[0].Length;
                        result = new List();
                        for(int i = 0; i < l; i++)
                            (result as List).Add(this.getFn((o as List)[index.Index[0][i]]));
                    }
                }

                else if(o is TArray) {
                    if(index.Rank > (o as TArray).Rank)
                        this.ManyIndexesError((o as TArray).Rank, index.Rank);

                    result = (o as TArray).GetSubArray(index.Index, index.Shrink, this.getFn);
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
        /// Function that provide getting
        /// </summary>
        private object EvaluateG(Guider guider, GetFunction getFn) {
            this.getFn = getFn;
            this.guider = guider;

            if(this.indexedItem is Indexer)
                return (this.indexedItem as Indexer).EvaluateG(guider, this.GetFn);
            else {
                return this.GetFn(EvaluateAtomObject(guider, this.indexedItem));
            }
        }

        /// <summary>
		/// Provede výpoèet funkce
		/// </summary>
        /// <param name="guider">Prùvodce výpoètu</param>
        /// <returns>Výsledek výpoètu</returns>
		public override object Evaluate(Guider guider) {
            return this.EvaluateG(guider, this.BasicGetFn);
		}

        /// <summary>
        /// Basic getting function
        /// </summary>
        private object BasicGetFn(object o) {
            return o;
        }
        #endregion

        /// <summary>
        /// Provede výpoèet všech indexù
        /// </summary>
        /// <param name="guider">Prùvodce výpoètem</param>
        /// <param name="o">Indexovaný object</param>
        private Indexes EvaluateIndexes(Guider guider, object o) {
            // Nejprve vypoèítáme všechny indexy
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
        /// Vrátí délku objektu v zadané hloubce
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
            else if(item is List)
                length = (item as List).Count;

            return length;
        }

		/// <summary>
		/// Nastaví na kontext pomocnou promìnnou s indexem posledního prvku øady
		/// </summary>
        /// <param name="context">Kontext, na kterém se spouští výpoèet</param>
        /// <param name="item">Objekt, jehož poslední index nastavujeme</param>
		/// <param name="depth">Hloubka</param>
		/// <returns>Jméno nastavené promìnné, jinak null</returns>
		private void SetEndVariable(Context context, object item, int depth) {
			string endVariable = this.endVariables[depth] as string;
			context.SetSystemVariable(endVariable, this.GetLength(item, depth) - 1);
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
		/// <param name="indexIn">Objekt s indexy</param>
		/// <param name="depth">Hloubka</param>
        /// <param name="indexOut">Výstup indexù</param>
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
		/// Chyba pøi špatném typu indexù
		/// </summary>
		/// <param name="needed">Poèet potøebných indexù</param>
		/// <param name="count">Poèet zadaných indexù</param>
		private object ManyIndexesError(int needed, int count) {
			throw new ExpressionException(string.Format(errorMessageManyIndexes, count, needed),
				string.Format(errorMessageDetail, this.expression));
		}

        private const string errorMessageBadIndexRank = "The indexing Array has {0} dimensions. Only 1 is required.";
		private const string errorMessageManyIndexes = "Indexer má pøíliš mnoho indexù. Zadáno: {0}, požadováno maximálnì {1}.";
		private const string errorMessageBadIndexType = "Index na pozici {0} má špatný typ '{1}'.";
		private const string errorMessageDetail = "Výraz: {0}";
	}
}
