using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    public delegate void FileNameEventHandler(object sender, FileNameEventArgs e);

    /// <summary>
    /// T��da k p�ed�v�n� jm�na souboru
    /// </summary>
    public class FileNameEventArgs: EventArgs {
        // Jm�no souboru
        private string fileName;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        public FileNameEventArgs(string fileName) {
            this.fileName = fileName;
        }

        /// <summary>
        /// Jm�no souboru
        /// </summary>
        public string FileName { get { return this.fileName; } }
    }

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
