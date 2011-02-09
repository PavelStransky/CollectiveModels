using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// P�ed�v�n� parametr� p�i exportov�n�
    /// </summary>
    public class ExportEventArgs: EventArgs {
        private string typeName;

        /// <summary>
        /// Jm�no pr�v� ukl�dan�ho typu
        /// </summary>
        public string TypeName { get { return this.typeName; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="typeName">Jm�no typu</param>
        public ExportEventArgs(string typeName) {
            this.typeName = typeName;
        }
    }
}
