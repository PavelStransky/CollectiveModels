using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Typy pro zv�razn�n�
    /// </summary>
    public enum HighlightTypes {
        Comment, 
        NormalBracket,
        IndexBracket, 
        Function,
        UserFunction, 
        Variable, 
        Number,
        String,
        EndVariable,
        Error, 
        Separator,
        Operator
    }

    /// <summary>
    /// Spravuje zv�razn�n� syntaxe
    /// </summary>
    public class Highlight: ArrayList {
        /// <summary>
        /// Pomocn� t��da s polo�kou zv�razn�n�
        /// </summary>
        public class HighlightItem {
            private HighlightTypes highlightType;
            private int start, end;
            private object comment;

            public HighlightItem(HighlightTypes highlightType, int start, int end, object comment) {
                this.highlightType = highlightType;
                this.start = start;
                this.end = end;
                this.comment = comment;
            }

            /// <summary>
            /// Nastav� funkci
            /// </summary>
            public void SetFunction() {
                if(this.highlightType == HighlightTypes.Variable && (this.comment as string)[0] == '_')
                    this.highlightType = HighlightTypes.UserFunction;
                else
                    this.highlightType = HighlightTypes.Function;
            }

            /// <summary>
            /// Nastav� chybu
            /// </summary>
            public void SetError(object comment) {
                this.highlightType = HighlightTypes.Error;
                this.comment = comment;
            }

            public HighlightTypes HighlightType { get { return this.highlightType; } }
            public int Start { get { return this.start; } }
            public int End { get { return this.end; } }
            public object Comment { get { return this.comment; } }

            public int Length { get { return this.end - this.start; } }
        }

        // Posledn� zv�raz�ovan� objekt
        private HighlightItem last;

        /// <summary>
        /// Vol�no, pokud byly nalezeny z�vorky;
        /// Pokud posledn� objekt byl text
        /// </summary>
        /// <param name="highlightType"></param>
        /// <returns></returns>
        public int BracketStart(HighlightTypes highlightType) {
            if(last != null && highlightType == HighlightTypes.NormalBracket) 
                last.SetFunction();
            last = null;
            return this.Count;
        }

        public void Add(HighlightTypes hightlightType, int start, int end, object comment) {
            HighlightItem item = new HighlightItem(hightlightType, start, end, comment);
            this.Add(item);

            if(hightlightType == HighlightTypes.Variable)
                last = item;
            else if(hightlightType != HighlightTypes.Comment)
                last = null;
        }

        public void Add(HighlightTypes highlightType, int start, object comment) {
            this.Add(highlightType, start, start + 1, comment);
        }

        public void Add(HighlightTypes highlightType, int start, int end) {
            this.Add(highlightType, start, end, null);
        }

        public void Add(HighlightTypes highlightType, int start) {
            this.Add(highlightType, start, null);
        }

        public void Add(int i, HighlightTypes highlightType, int start, int end) {
            this.Insert(i, new HighlightItem(highlightType, start, end, null));
        }

        /// <summary>
        /// K z�vorce nalezne pozici druh� z�vorky; pokud nic nenalezne, vr�t� 0
        /// </summary>
        /// <param name="pos">Pozice prvn� z�vorky</param>
        public HighlightItem FindSecondBracket(int pos) {
            foreach(HighlightItem item in this)
                if(item.HighlightType == HighlightTypes.NormalBracket
                    || item.HighlightType == HighlightTypes.IndexBracket) {
                    if(item.Start == pos || item.End == pos)
                        return item;
                }

            return null;
        }

        /// <summary>
        /// Vyvol� v�jimky zp�soben� chybami
        /// </summary>
        public void ThrowErrors() {
            foreach(HighlightItem item in this)
                if(item.HighlightType == HighlightTypes.Error)
                    throw item.Comment as Exception;
        }

        /// <summary>
        /// Zkontroluje syntaxi v cel�m textu
        /// </summary>
        /// <param name="e">V�raz</param>
        public void CheckSyntax(string e) {
            this.Clear();
            Atom.CheckSyntax(e, this);
        }

        /// <summary>
        /// Z�kladn� p�smo pro zv�razn�n�
        /// </summary>
        private Font baseFont;
        private Font boldFont;

        public Font BaseFont {
            get {
                return this.baseFont;
            }
            set {
                this.baseFont = value;
                this.boldFont = new Font(value, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Z�kladn� barva
        /// </summary>
        public Color DefaultColor { get { return Color.Blue; } }

        public void HighlightAll(IHighlightText h) {
            HighlightTypes lastType = HighlightTypes.Separator;

            foreach(HighlightItem item in this) {
                if(item.HighlightType == HighlightTypes.IndexBracket
                    || item.HighlightType == HighlightTypes.Number
                    || item.HighlightType == HighlightTypes.Operator
                    || item.HighlightType == HighlightTypes.Variable)
                    continue;

                if(item.HighlightType == HighlightTypes.NormalBracket) {
                    if(lastType == HighlightTypes.Function) {
                        h.SelectionStart = item.Start;
                        h.SelectionLength = 1;
                        h.SelectionFont = this.boldFont;
                        h.SelectionColor = Color.Black;

                        h.SelectionStart = item.End;
                        h.SelectionLength = 1;
                        h.SelectionFont = this.boldFont;
                        h.SelectionColor = Color.Black;
                    }
                    else if(lastType == HighlightTypes.UserFunction) {
                        h.SelectionStart = item.Start;
                        h.SelectionLength = 1;
                        h.SelectionFont = this.boldFont;
                        h.SelectionColor = Color.Gray;

                        h.SelectionStart = item.End;
                        h.SelectionLength = 1;
                        h.SelectionFont = this.boldFont;
                        h.SelectionColor = Color.Gray;
                    }

                    lastType = item.HighlightType;
                    continue;
                }

                h.SelectionStart = item.Start;
                h.SelectionLength = item.Length;

                if(item.HighlightType == HighlightTypes.Comment)
                    h.SelectionColor = Color.Gray;

                else if(item.HighlightType == HighlightTypes.EndVariable)
                    h.SelectionColor = Color.Black;

                else if(item.HighlightType == HighlightTypes.Error) {
                    h.SelectionFont = this.boldFont;
                    h.SelectionColor = Color.Red;
                }

                else if(item.HighlightType == HighlightTypes.Function) {
                    h.SelectionFont = boldFont;
                    h.SelectionColor = Color.Black;
                }

                else if(item.HighlightType == HighlightTypes.UserFunction) {
                    h.SelectionFont = this.boldFont;
                    h.SelectionColor = Color.Gray;
                }

                else if(item.HighlightType == HighlightTypes.Separator) {
                    h.SelectionFont = this.boldFont;
                    h.SelectionColor = Color.Black;
                }

                else if(item.HighlightType == HighlightTypes.String)
                    h.SelectionColor = Color.Green;

                lastType = item.HighlightType;
            }
        }

        /// <summary>
        /// Najde prvek, uvnit� n�ho� co nejl�pe sed� dan� index
        /// </summary>
        /// <param name="index">Index</param>
        public HighlightItem FindItem(int index) {
            HighlightItem result = null;

            foreach(HighlightItem item in this)
                if(item.Start <= index && item.End >= index)
                    result = item;
                else
                    if(result != null && item.Start > index)
                        break;

            return result;
        }
    }
}
