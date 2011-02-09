using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Pøedávání parametrù pøi exportování
    /// </summary>
    public class ExportEventArgs: EventArgs {
        private string typeName;

        /// <summary>
        /// Jméno právì ukládaného typu
        /// </summary>
        public string TypeName { get { return this.typeName; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="typeName">Jméno typu</param>
        public ExportEventArgs(string typeName) {
            this.typeName = typeName;
        }
    }
}
