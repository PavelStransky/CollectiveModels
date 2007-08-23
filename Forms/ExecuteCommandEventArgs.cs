using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// Pøedávání parametrù pøi události spuštìní pøíkazu
    /// </summary>
    public class ExecuteCommandEventArgs: EventArgs {
        private string expression;
        private bool newWindow;

        /// <summary>
        /// Výraz k provedení
        /// </summary>
        public string Expression { get { return this.expression; } }

        /// <summary>
        /// True, pokud pøi je požadováno otevøení nového okna
        /// </summary>
        public bool NewWindow { get { return this.newWindow; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz k provedení</param>
        /// <param name="newWindow">Nové okno</param>
        public ExecuteCommandEventArgs(string expression, bool newWindow) {
            this.expression = expression;
            this.newWindow = newWindow;
        }
    }
}
