using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using PavelStransky.Core;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Základní pøedek pro graf
	/// </summary>
	public partial class GraphControl : System.Windows.Forms.UserControl {
        private IGraphControl graphControl;

        /// <summary>
        /// Vrátí promìnnou s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graphControl != null ? this.graphControl.GetGraph() : null;
        }

		/// <summary>
		/// Nastaví promìnnou s daty
		/// </summary>
		/// <param name="graph">Promìnná s grafem</param>
		public void SetGraph(Graph graph) {
			if(graph.Item as IExportable != null)
				this.cmnSaveAsTxt.Enabled = true;
			else
				this.cmnSaveAsTxt.Enabled = false;

            this.lblTitle.Text = (string)graph.GetGeneralParameter(paramTitle, defaultTitle);
            this.toolTip.RemoveAll();

            if(this.graphControl == null) {
                GraphicsBox graphicsBox = new GraphicsBox();
                graphicsBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                graphicsBox.Location = new Point(0, this.lblTitle.Height);
                graphicsBox.Size = new Size(this.Width, this.Height - graphicsBox.Top);
                graphicsBox.BackColor = System.Drawing.SystemColors.Window;
                graphicsBox.TabIndex = 0;
                graphicsBox.TabStop = false;
                graphicsBox.MouseMove += new MouseEventHandler(graphControl_MouseMove);
                graphicsBox.MouseDown += new MouseEventHandler(graphicsBox_MouseDown);
                this.Controls.Add(graphicsBox);
                this.graphControl = graphicsBox;
            }

            this.graphControl.SetGraph(graph);
        }

        /// <summary>
        /// Pro nastavení toolTipu
        /// </summary>
        void graphControl_MouseMove(object sender, MouseEventArgs e) {
            this.toolTip.SetToolTip(this.graphControl as Control, this.graphControl.ToolTip(e.X, e.Y));
        }

        /// <summary>
        /// Pro drag & drop akci
        /// </summary>
        void graphicsBox_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left && e.Clicks == 1)
                this.DoDragDrop(this.graphControl.CoordinatesFromPosition(e.X, e.Y), DragDropEffects.Copy);
        }

		/// <summary>
		/// Základní konstruktor (pro designer)
		/// </summary>
		public GraphControl() {
			this.InitializeComponent();
			this.lblTitle.Width = this.Width;
			this.lblTitle.Height = 24;

            this.saveFileDialogTxt.Filter = WinMain.FileFilterTxt;
            this.saveFileDialogTxt.DefaultExt = WinMain.FileExtTxt;
            this.saveFileDialogGif.Filter = WinMain.FileFilterGif;
            this.saveFileDialogGif.DefaultExt = WinMain.FileExtGif;
            this.saveFileDialogPicture.Filter = WinMain.FileFilterPicture;
            this.saveFileDialogPicture.DefaultExt = WinMain.FileExtPicture;
        }

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="graph">Data s grafem</param>
		public GraphControl(Graph graph) : this() {
			this.SetGraph(graph);
		}

        /// <summary>
        /// Vypne kontextové menu (aby neprobíhaly pøes sebe dvì akce zároveò)
        /// </summary>
        public void DisableMenu() {
            this.cmnSaveAsGif.Enabled = false;
        }

        /// <summary>
        /// Zobrazí kontextové menu
        /// </summary>
        public void EnableMenu() {
            this.cmnSaveAsGif.Enabled = true;
        }

		/// <summary>
		/// Položka menu Uložit jako text...
		/// </summary>
		private void cmnSaveAsTxt_Click(object sender, System.EventArgs e) {
            this.saveFileDialogTxt.InitialDirectory = WinMain.Directory;
            if(this.saveFileDialogTxt.FileName == string.Empty) 
                this.saveFileDialogTxt.FileName = this.Parent.Text;

			this.saveFileDialogTxt.ShowDialog();
		}	

		/// <summary>
		/// Uložení dat
		/// </summary>
		private void saveFileDialogTxt_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetDirectoryFromFile(this.saveFileDialogTxt.FileName);
            Export export = new Export(this.saveFileDialogTxt.FileName, false);
            export.Write(this.GetGraph());
            export.Close();
		}

        /// <summary>
        /// Položka menu Uložit jako GIF...
        /// </summary>
        private void cmnSaveAsGif_Click(object sender, EventArgs e) {
            this.saveFileDialogGif.InitialDirectory = WinMain.Directory;
            if(this.saveFileDialogGif.FileName == string.Empty)
                this.saveFileDialogGif.FileName = this.Parent.Text;

            this.saveFileDialogGif.ShowDialog();
        }

        /// <summary>
        /// Uložení obrázku
        /// </summary>
        private void saveFileDialogGif_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetDirectoryFromFile(this.saveFileDialogGif.FileName);
            this.graphControl.SaveGIF(this.saveFileDialogGif.FileName);
        }

        /// <summary>
        /// Položka menu Uložit jako obrázek...
        /// </summary>
        private void cmnSaveAsPicture_Click(object sender, EventArgs e) {
            this.saveFileDialogPicture.InitialDirectory = WinMain.Directory;
            if(this.saveFileDialogPicture.FileName == string.Empty)
                this.saveFileDialogPicture.FileName = this.Parent.Text;

            this.saveFileDialogPicture.ShowDialog();
        }

        /// <summary>
        /// Uložení obrázku sekvenènì
        /// </summary>
        private void saveFileDialogPicture_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetDirectoryFromFile(this.saveFileDialogPicture.FileName);
            this.graphControl.SavePicture(this.saveFileDialogPicture.FileName);
        }

        private const string paramTitle = "title";
        private const string defaultTitle = "";
	}
}