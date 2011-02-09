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

        // True, pokud se ulo�en� poda�ilo
        private bool success;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        /// <param name="success">True, pokud se ulo�en� poda�ilo</param>
        public FileNameEventArgs(string fileName, bool success) {
            this.fileName = fileName;
            this.success = success;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        public FileNameEventArgs(string fileName)
            : this(fileName, false) { }

        /// <summary>
        /// Jm�no souboru
        /// </summary>
        public string FileName { get { return this.fileName; } }

        /// <summary>
        /// True, pokud se ulo�en� poda�ilo
        /// </summary>
        public bool Success { get { return this.success; } }
    }
}
