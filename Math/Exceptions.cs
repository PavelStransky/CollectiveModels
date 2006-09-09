using System;

namespace PavelStransky.Math {
	/// <summary>
	/// V�jimka s detailem
	/// </summary>
	public class DetailException: ApplicationException {
		private string detailMessage = string.Empty;

		/// <summary>
		/// P��davn� informace o v�jimce
		/// </summary>
		public string DetailMessage {get {return this.detailMessage;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public DetailException(string message) : base(message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public DetailException(string message, Exception innerException) : base(message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		/// <param name="detailMessage">Detail chyby</param>
		public DetailException(string message, string detailMessage) : this(message) {
			this.detailMessage = detailMessage;
		}
	}

	/// <summary>
	/// V�jimka p�i implementaci IExportable
	/// </summary>
	public class ImportExportException: DetailException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ImportExportException(string message) : base(message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public ImportExportException(string message, Exception innerException) : base(message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		/// <param name="detailMessage">Detail chyby</param>
		public ImportExportException(string message, string detailMessage) : base(message, detailMessage) {}

		/// <summary>
		/// Provede kontrolu typ�
		/// </summary>
		/// <param name="fType">Typ dat v souboru</param>
		/// <param name="cType">Typ t��dy</param>
		public static void CheckImportType(string fType, Type cType) {
			if(fType.Trim() != cType.FullName)
				throw new ImportExportException(errorMessageImportBadType,
					string.Format(errorMessageImportBadTypeDetail, fType, cType));
		}

		private const string errorMessageImportBadType = "Nesouhlas� typy p�i importu souboru dat.";
		private const string errorMessageImportBadTypeDetail = "Typ dat souboru: {0}\nTyp t��dy: {1}";
	}
}
