using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// Tøída pro pøedávání informací o skonèeném výpoètu
    /// </summary>
    public class FinishedEventArgs: EventArgs {
        private bool successful;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="successful"></param>
        public FinishedEventArgs(bool successful) {
            this.successful = successful;
        }

        /// <summary>
        /// True, pokud byl výpoèet ukonèen úspìchem
        /// </summary>
        public bool Successful { get { return this.successful; } }
    }
}
