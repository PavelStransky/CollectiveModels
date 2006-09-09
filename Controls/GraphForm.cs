using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PavelStransky.Controls {
	/// <summary>
	/// Pr�zdn� formul��. Grafy do n�j p�id�v�me my
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
		/// Pokud existuje, najde na na formul��i graf s dan�m
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
		/// Vr�t� po�et control� typu Graph, kter� se vyskytuj� na formul��i str�nce
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
		/// P�i zm�n� velikosti formul��e mus�me zm�nit velikosti v�ech graf�
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
	/// V�jimka ve t��d� Graph
	/// </summary>
	public class GraphException: ApplicationException {
		private string detailMessage = string.Empty;

		/// <summary>
		/// P��davn� informace o v�jimce
		/// </summary>
		public string DetailMessage {get {return this.detailMessage;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public GraphException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public GraphException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		/// <param name="detailMessage">Detail chyby</param>
		public GraphException(string message, string detailMessage) : this(message) {
			this.detailMessage = detailMessage;
		}

		private const string errMessage = "V grafu do�lo k chyb�: ";
	}
}
