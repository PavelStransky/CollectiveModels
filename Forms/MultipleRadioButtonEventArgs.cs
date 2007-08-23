using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    public class MultipleRadioButtonEventArgs: EventArgs {
        private string name;

        /// <summary>
        /// Jm�no RadioButtonu, na kter� bylo kliknuto
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Jm�no buttonu, na kter� bylo kliknuto</param>
        public MultipleRadioButtonEventArgs(string name) {
            this.name = name;
        }
    }
}
