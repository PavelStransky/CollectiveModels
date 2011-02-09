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

        // True, pokud se uložení podaøilo
        private bool success;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        /// <param name="success">True, pokud se uložení podaøilo</param>
        public FileNameEventArgs(string fileName, bool success) {
            this.fileName = fileName;
            this.success = success;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        public FileNameEventArgs(string fileName)
            : this(fileName, false) { }

        /// <summary>
        /// Jméno souboru
        /// </summary>
        public string FileName { get { return this.fileName; } }

        /// <summary>
        /// True, pokud se uložení podaøilo
        /// </summary>
        public bool Success { get { return this.success; } }
    }
}
