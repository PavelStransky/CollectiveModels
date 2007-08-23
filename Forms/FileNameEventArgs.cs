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
}
