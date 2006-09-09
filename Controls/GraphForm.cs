using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PavelStransky.Controls {
	/// <summary>
	/// Prázdný formuláø. Grafy do nìj pøidáváme my
	/// </summary>
	public class GraphForm : ContextForm {
		/// <summary>
		/// Konstruktor
		/// </summary>
        public GraphForm()
            : base() {
        }

		/// <summary>
		/// Konstruktor s kontextem
		/// </summary>
		/// <param name="context">Kontext</param>
        public GraphForm(Controls.Context context)
            : base(context) {
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
		protected override void InitializeComponent() {
			// 
			// GraphForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(720, 622);
			this.Name = "GraphForm";
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
