using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Rozhraní pro control grafu
    /// </summary>
    public interface IGraphControl {
        /// <summary>
        /// Nastaví graf
        /// </summary>
        /// <param name="graph">Objekt s grafem</param>
        void SetGraph(Graph graph);

        /// <summary>
        /// Naète graf
        /// </summary>
        /// <param name="graph"></param>
        Graph GetGraph();

        /// <summary>
        /// Vrátí text ToolTipu
        /// </summary>
        /// <param name="x">X-ová souøadnice</param>
        /// <param name="y">Y-ová souøadnice</param>
        string ToolTip(int x, int y);

        /// <summary>
        /// Uloží graf jako GIF
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        void SaveGIF(string fName);
    }

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

            if(graph.IsLineGraph) {
                if(this.graphControl is LineBox) {
                    this.graphControl.SetGraph(graph);
                }
                else {
                    this.SuspendLayout();
                    if(this.Controls.Contains(this.graphControl as Control))
                        this.Controls.Remove(this.graphControl as Control);
                    
                    LineBox lineBox = new LineBox();
                    lineBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                    lineBox.Location = new Point(0, this.lblTitle.Height);
                    lineBox.Size = new Size(this.Width, this.Height - lineBox.Top);
                    lineBox.BackColor = System.Drawing.SystemColors.Window;
                    lineBox.TabIndex = 0;
                    lineBox.TabStop = false;
                    lineBox.MouseMove += new MouseEventHandler(graphControl_MouseMove);
                    this.Controls.Add(lineBox);
                    this.graphControl = lineBox;
                    lineBox.SetGraph(graph);

                    this.ResumeLayout();
                }
            }
            else if(graph.IsDensityGraph) {
                if(this.graphControl is DensityBox) {
                    this.graphControl.SetGraph(graph);
                }
                else {
                    this.SuspendLayout();
                    if(this.Controls.Contains(this.graphControl as Control))
                        this.Controls.Remove(this.graphControl as Control);
                    
                    DensityBox densityBox = new DensityBox();
                    densityBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                    densityBox.Location = new Point(0, this.lblTitle.Height);
                    densityBox.Size = new Size(this.Width, this.Height - densityBox.Top);
                    densityBox.BackColor = System.Drawing.SystemColors.Window;
                    densityBox.TabIndex = 0;
                    densityBox.TabStop = false;
                    densityBox.MouseMove += new MouseEventHandler(graphControl_MouseMove);
                    this.Controls.Add(densityBox);
                    this.graphControl = densityBox;
                    densityBox.SetGraph(graph);

                    this.ResumeLayout();
                }
            }

        }

        /// <summary>
        /// Pro nastavení toolTipu
        /// </summary>
        void graphControl_MouseMove(object sender, MouseEventArgs e) {
            this.toolTip.SetToolTip(this.graphControl as Control, this.graphControl.ToolTip(e.X, e.Y));
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

        private const string paramTitle = "title";
        private const string defaultTitle = "";
	}
}