using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// P�ed�v�n� parametr� p�i ud�losti spu�t�n� p��kazu
    /// </summary>
    public class ExecuteCommandEventArgs: EventArgs {
        private string expression;
        private bool newWindow;

        /// <summary>
        /// V�raz k proveden�
        /// </summary>
        public string Expression { get { return this.expression; } }

        /// <summary>
        /// True, pokud p�i je po�adov�no otev�en� nov�ho okna
        /// </summary>
        public bool NewWindow { get { return this.newWindow; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz k proveden�</param>
        /// <param name="newWindow">Nov� okno</param>
        public ExecuteCommandEventArgs(string expression, bool newWindow) {
            this.expression = expression;
            this.newWindow = newWindow;
        }
    }
}
