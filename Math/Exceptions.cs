using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Výjimka s detailem
	/// </summary>
	public class DetailException: ApplicationException {
		private string detailMessage = string.Empty;

		/// <summary>
		/// Pøídavné informace o výjimce
		/// </summary>
		public string DetailMessage {get {return this.detailMessage;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public DetailException(string message) : base(message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public DetailException(string message, Exception innerException) : base(message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		/// <param name="detailMessage">Detail chyby</param>
		public DetailException(string message, string detailMessage) : this(message) {
            this.detailMessage = detailMessage;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public DetailException(string message, string detailMessage, Exception innerException)
            : this(message, innerException) {
            this.detailMessage = detailMessage;
        }
	}

	/// <summary>
	/// Výjimka pøi implementaci IExportable
	/// </summary>
	public class ImportExportException: DetailException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ImportExportException(string message) : base(message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public ImportExportException(string message, Exception innerException) : base(message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		/// <param name="detailMessage">Detail chyby</param>
		public ImportExportException(string message, string detailMessage) : base(message, detailMessage) {}
	}
}
