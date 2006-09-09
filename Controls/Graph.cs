using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Controls {
	/// <summary>
	/// Základní pøedek pro graf
	/// </summary>
	public class Graph : System.Windows.Forms.UserControl {
		private System.Windows.Forms.Label lblTitle;
		private System.ComponentModel.IContainer components = null;
		protected System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem cmnSaveAs;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.MenuItem cmnRefresh;

		// Promìnná s daty
		private Variable variable;
		private int index;
		private Expression.Graph graphItem;
		private RectangleF position = new RectangleF(0, 0, 1, 1);

		/// <summary>
		/// Index grafu
		/// </summary>
		public int Index {get {return this.index;}}

		/// <summary>
		/// Nastaví promìnnou s daty
		/// </summary>
		/// <param name="variable">Promìnné s daty</param>
		public void SetVariable(Variable variable) {
			this.index = -1;

			this.variable = variable;
			if(this.variable.Item is Expression.Graph)
				this.graphItem = this.variable.Item as Expression.Graph;
			else
				this.graphItem = null;

			if(this.Item is IExportable)
				this.cmnSaveAs.Enabled = true;
			else
				this.cmnSaveAs.Enabled = false;

			this.ShowGraph();
		}

		/// <summary>
		/// Nastaví promìnnou s daty
		/// </summary>
		/// <param name="variable">Promìnná s daty ve formì GraphArray</param>
		/// <param name="index">Index</param>
		public void SetVariable(Variable variable, int index) {
			if(index == -1) {
				this.SetVariable(variable);
				return;
			}

			this.variable = variable;
			this.index = index;

			if(this.variable.Item is GraphArray)
				this.graphItem = (this.variable.Item as GraphArray)[index];
			else
				this.graphItem = null;

			if(this.Item is IExportable)
				this.cmnSaveAs.Enabled = true;
			else
				this.cmnSaveAs.Enabled = false;

			this.ShowGraph();
		}

		/// <summary>
		/// Data k vykreslení
		/// </summary>
		protected object Item {
			get {
				if(this.graphItem == null)
					return null;
				else
					return this.graphItem.Item;
			}
		}

		/// <summary>
		/// Graf
		/// </summary>
		protected Expression.Graph GraphItem {get {return this.graphItem;}}

		/// <summary>
		/// Základní konstruktor (pro designer)
		/// </summary>
		public Graph() {
			InitializeComponent();
			this.lblTitle.Width = this.Width;
			this.lblTitle.Height = 24;

			this.saveFileDialog.Filter = defaultFileFilter;
			this.saveFileDialog.DefaultExt = defaultFileExt;
			this.saveFileDialog.InitialDirectory = defaultDirectory;
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		public Graph(Variable variable) : this() {
			this.SetVariable(variable);
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		/// <param name="index">Index pro GraphArray</param>
		public Graph(Variable variable, int index) : this() {
			this.SetVariable(variable, index);
		}

		public Graph(Variable variable, int index, RectangleF position) : this(variable, index) {
			this.position = position;
		}

		/// <summary>
		/// Zobrazí graf
		/// </summary>
		protected virtual void ShowGraph() {
			if(this.graphItem != null)
				this.lblTitle.Text = this.graphItem.Title;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		protected virtual void InitializeComponent() {
			this.lblTitle = new System.Windows.Forms.Label();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.cmnSaveAs = new System.Windows.Forms.MenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.cmnRefresh = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
			this.lblTitle.Location = new System.Drawing.Point(0, 0);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(150, 24);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.cmnRefresh,
																						this.cmnSaveAs});
			// 
			// cmnSaveAs
			// 
			this.cmnSaveAs.Index = 1;
			this.cmnSaveAs.Text = "&Uložit jako...";
			this.cmnSaveAs.Click += new System.EventHandler(this.cmnSaveAs_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
			// 
			// cmnRefresh
			// 
			this.cmnRefresh.Index = 0;
			this.cmnRefresh.Text = "&Obnovit";
			this.cmnRefresh.Click += new System.EventHandler(this.cmnRefresh_Click);
			// 
			// Graph
			// 
			this.ContextMenu = this.contextMenu;
			this.Controls.Add(this.lblTitle);
			this.Name = "Graph";
			this.Size = new System.Drawing.Size(150, 130);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Položka menu Uložit jako...
		/// </summary>
		private void cmnSaveAs_Click(object sender, System.EventArgs e) {
			this.saveFileDialog.ShowDialog();
		}	

		/// <summary>
		/// Uložení dat
		/// </summary>
		private void saveFileDialog_FileOk(object sender, CancelEventArgs e) {
			(this.Item as IExportable).Export(this.saveFileDialog.FileName, true);
		}

		/// <summary>
		/// Vypoèítá výraz v promìnné a znovu zobrazí výsledek
		/// </summary>
		private void cmnRefresh_Click(object sender, System.EventArgs e) {
			try {
				this.variable.Evaluate();
			}
			finally {
				this.ShowGraph();
			}
		}

		/// <summary>
		/// Nastaví pozici grafu vùèi rodièi
		/// </summary>
		/// <param name="width">Šíøka rodièe</param>
		/// <param name="height">Výška rodièe</param>
		public void SetPosition(int width, int height) {
			this.Left = (int)(width * this.position.Left) + graphMargin;
			this.Width = (int)(width * this.position.Width) - 2 * graphMargin;
			this.Top = (int)(height * this.position.Top) + graphMargin;
			this.Height = (int)(height * this.position.Height) - 2 * graphMargin;
		}

		private const string defaultFileFilter = "Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";
		private const string defaultFileExt = "txt";
		private const string defaultDirectory = "c:\\gcm";

		private const int graphMargin = 8;
	}
}