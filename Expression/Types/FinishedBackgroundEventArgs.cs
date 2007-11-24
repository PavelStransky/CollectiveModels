using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída k pøedávání informací pøi ukonèení výpoètu jednotlivého pozadí
    /// </summary>
    public class FinishedBackgroundEventArgs: EventArgs {
        private int group;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="group">Èíslo skupiny, pro kterou bylo pozadí dopoèítáno</param>
        public FinishedBackgroundEventArgs(int group) {
            this.group = group;
        }

        /// <summary>
        /// Èíslo skupiny, pro kterou bylo pozadí dopoèítáno
        /// </summary>
        public int Group { get { return this.group; } }
    }
}
