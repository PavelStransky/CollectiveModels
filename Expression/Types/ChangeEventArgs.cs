using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// T��da k p�ed�v�n� informac� p�i zm�n� prom�nn�
    /// </summary>
    public class ChangeEventArgs: EventArgs {
        private Context context;
        // P�vodn� objekt
        private object oldItem;
        // Nov� objekt
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
