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
}
