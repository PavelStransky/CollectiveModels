using System;
using System.Collections;
using System.Text;

using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Tøída k pøedávání zvýraznìného objektu
    /// </summary>
    public class HighlightItemEventArgs: EventArgs {
        private Highlight.HighlightItem highlightItem;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public HighlightItemEventArgs(Highlight.HighlightItem highlightItem) {
            this.highlightItem = highlightItem;
        }

        public Highlight.HighlightItem HighlightItem { get { return this.highlightItem; } }
    }
}
