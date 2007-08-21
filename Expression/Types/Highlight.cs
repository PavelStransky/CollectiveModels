using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Typy pro zvýraznìní
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
    /// Spravuje zvýraznìní syntaxe
    /// </summary>
    public class Highlight: ArrayList {
        /// <summary>
        /// Pomocná tøída s položkou zvýraznìní
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
            /// Nastaví funkci
            /// </summary>
            public void SetFunction() {
                if((this.comment as string)[0] == '_')
                    this.highlightType = HighlightTypes.UserFunction;
                else
                    this.highlightType = HighlightTypes.Function;
            }

            /// <summary>
            /// Nastaví chybu
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

        private HighlightItem last;

        public int BracketStart(HighlightTypes highlightType) {
            if(last != null && highlightType == HighlightTypes.NormalBracket)
                last.SetFunction();
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
        /// K závorce nalezne pozici druhé závorky; pokud nic nenalezne, vrátí 0
        /// </summary>
        /// <param name="pos">Pozice první závorky</param>
        public HighlightItem FindSecondBracket(int pos) {
            foreach(HighlightItem item in this)
                if(item.HighlightType == HighlightTypes.NormalBracket
                    || item.HighlightType == HighlightTypes.IndexBracket) {
                    if(item.Start == pos || item.End == pos)
                        return item;
                }

            return null;
        }
    }
}
