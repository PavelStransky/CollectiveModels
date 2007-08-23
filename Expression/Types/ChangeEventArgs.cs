using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída k pøedávání informací pøi zmìnì promìnné
    /// </summary>
    public class ChangeEventArgs: EventArgs {
        private Context context;
        // Pùvodní objekt
        private object oldItem;
        // Nový objekt
        private object newItem;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ChangeEventArgs(Context context, object oldItem, object newItem) {
            this.context = context;
            this.oldItem = oldItem;
            this.newItem = newItem;
        }

        public Context Context { get { return this.context; } }
        public object OldItem { get { return this.oldItem; } }
        public object NewItem { get { return this.newItem; } }
    }
}
