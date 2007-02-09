using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    public delegate void FileNameEventHandler(object sender, FileNameEventArgs e);

    /// <summary>
    /// Tøída k pøedávání jména souboru
    /// </summary>
    public class FileNameEventArgs: EventArgs {
        // Jméno souboru
        private string fileName;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        public FileNameEventArgs(string fileName) {
            this.fileName = fileName;
        }

        /// <summary>
        /// Jméno souboru
        /// </summary>
        public string FileName { get { return this.fileName; } }
    }

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
