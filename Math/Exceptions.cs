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

		/// <summary>
		/// Provede kontrolu typù
		/// </summary>
		/// <param name="fType">Typ dat v souboru</param>
		/// <param name="cType">Typ tøídy</param>
		public static void CheckImportType(string fType, Type cType) {
			if(fType.Trim() != cType.FullName)
				throw new ImportExportException(errorMessageImportBadType,
					string.Format(errorMessageImportBadTypeDetail, fType, cType));
		}

		private const string errorMessageImportBadType = "Nesouhlasí typy pøi importu souboru dat.";
		private const string errorMessageImportBadTypeDetail = "Typ dat souboru: {0}\nTyp tøídy: {1}";
	}
}
