using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    public class MultipleRadioButtonEventArgs: EventArgs {
        private string name;

        /// <summary>
        /// Jméno RadioButtonu, na který bylo kliknuto
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Jméno buttonu, na který bylo kliknuto</param>
        public MultipleRadioButtonEventArgs(string name) {
            this.name = name;
        }
    }
}
