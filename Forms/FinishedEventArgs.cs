using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// T��da pro p�ed�v�n� informac� o skon�en�m v�po�tu
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
        /// True, pokud byl v�po�et ukon�en �sp�chem
        /// </summary>
        public bool Successful { get { return this.successful; } }
    }
}
