using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using PavelStransky.Math;

namespace PavelStransky.Forms {
	/// <summary>
	/// Prázdný formuláø. Grafy do nìj pøidáváme my
	/// </summary>
	public class GraphForm : ChildForm, IExportable {
		/// <summary>
		/// Konstruktor
		/// </summary>
        public GraphForm() {
            this.InitializeComponent();
        }

		/// <summary>
		/// Pokud existuje, najde na na formuláøi graf s daným
		/// </summary>
		/// <param name="index">Index</param>
		public Graph GraphControl(int index) {
			foreach(Control control in this.Controls)
				if((control as Graph) != null) 
					if((control as Graph).Index == index)
						return control as Graph;

			return null;
		}

		/// <summary>
		/// Vrátí poèet controlù typu Graph, které se vyskytují na formuláøi stránce
		/// </summary>
		public int NumGraphControls() {
			int result = 0;

			foreach(Control control in this.Controls)
				if((control as Graph) != null)
					result++;

			return result;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		protected void InitializeComponent() {
            this.SuspendLayout();
            // 
            // GraphForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(392, 266);
            this.Name = "GraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Pøi zmìnì velikosti formuláøe musíme zmìnit velikosti všech grafù
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged (e);

			foreach(Control control in this.Controls) {
				Graph g = control as Graph;
				if(g != null) 
					g.SetPosition(this.Width, this.Height);
			}
		}

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah formuláøe do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            // Musíme ukládat binárnì
            if(!export.Binary)
                throw new Exception("");

            // Binárnì
            BinaryWriter b = export.B;
            b.Write(this.Location.X);
            b.Write(this.Location.Y);
            b.Write(this.Size.Width);
            b.Write(this.Size.Height);

            b.Write(this.Name);
        }

        /// <summary>
        /// Naète obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            // Musíme èíst binárnì
            if(!import.Binary)
                throw new Exception("");

            // Binárnì
            BinaryReader b = import.B;
            this.Location = new Point(b.ReadInt32(), b.ReadInt32());
            this.Size = new Size(b.ReadInt32(), b.ReadInt32());

            this.Name = b.ReadString();
            this.Text = this.Name;
        }
        #endregion
    }

	/// <summary>
	/// Výjimka ve tøídì Graph
	/// </summary>
	public class GraphException: ApplicationException {
		private string detailMessage = string.Empty;

		/// <summary>
		/// Pøídavné informace o výjimce
		/// </summary>
		public string DetailMessage {get {return this.detailMessage;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public GraphException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public GraphException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		/// <param name="detailMessage">Detail chyby</param>
		public GraphException(string message, string detailMessage) : this(message) {
			this.detailMessage = detailMessage;
		}

		private const string errMessage = "V grafu došlo k chybì: ";
	}
}
