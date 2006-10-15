using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Rozhran� pro control grafu
    /// </summary>
    public interface IGraphControl {
        /// <summary>
        /// Nastav� graf
        /// </summary>
        /// <param name="graph">Objekt s grafem</param>
        void SetGraph(Graph graph);

        /// <summary>
        /// Na�te graf
        /// </summary>
        /// <param name="graph"></param>
        Graph GetGraph();

        /// <summary>
        /// Vr�t� text ToolTipu
        /// </summary>
        /// <param name="x">X-ov� sou�adnice</param>
        /// <param name="y">Y-ov� sou�adnice</param>
        string ToolTip(int x, int y);
    }

	/// <summary>
	/// Z�kladn� p�edek pro graf
	/// </summary>
	public partial class GraphControl : System.Windows.Forms.UserControl {
        private IGraphControl graphControl;

        /// <summary>
        /// Vr�t� prom�nnou s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graphControl != null ? this.graphControl.GetGraph() : null;
        }

		/// <summary>
		/// Nastav� prom�nnou s daty
		/// </summary>
		/// <param name="graph">Prom�nn� s grafem</param>
		public void SetGraph(Graph graph) {
			if(graph.Item as IExportable != null)
				this.cmnSaveAs.Enabled = true;
			else
				this.cmnSaveAs.Enabled = false;

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
                    
                    LineBox lineBox = new LineBox(graph);
                    lineBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                    lineBox.Location = new Point(0, this.lblTitle.Height);
                    lineBox.Size = new Size(this.Width, this.Height - lineBox.Top);
                    lineBox.BackColor = System.Drawing.SystemColors.Window;
                    lineBox.TabIndex = 0;
                    lineBox.TabStop = false;
                    lineBox.MouseMove += new MouseEventHandler(graphControl_MouseMove);
                    this.Controls.Add(lineBox);
                    this.graphControl = lineBox;

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
                    
                    DensityBox densityBox = new DensityBox(graph);
                    densityBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                    densityBox.Location = new Point(0, this.lblTitle.Height);
                    densityBox.Size = new Size(this.Width, this.Height - densityBox.Top);
                    densityBox.BackColor = System.Drawing.SystemColors.Window;
                    densityBox.TabIndex = 0;
                    densityBox.TabStop = false;
                    densityBox.MouseMove += new MouseEventHandler(graphControl_MouseMove);
                    this.Controls.Add(densityBox);
                    this.graphControl = densityBox;

                    this.ResumeLayout();
                }
            }

        }

        /// <summary>
        /// Pro nastaven� toolTipu
        /// </summary>
        void graphControl_MouseMove(object sender, MouseEventArgs e) {
            this.toolTip.SetToolTip(this.graphControl as Control, this.graphControl.ToolTip(e.X, e.Y));
        }

		/// <summary>
		/// Z�kladn� konstruktor (pro designer)
		/// </summary>
		public GraphControl() {
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
		/// <param name="graph">Data s grafem</param>
		public GraphControl(Graph graph) : this() {
			this.SetGraph(graph);
		}

		/// <summary>
		/// Polo�ka menu Ulo�it jako...
		/// </summary>
		private void cmnSaveAs_Click(object sender, System.EventArgs e) {
			this.saveFileDialog.ShowDialog();
		}	

		/// <summary>
		/// Ulo�en� dat
		/// </summary>
		private void saveFileDialog_FileOk(object sender, CancelEventArgs e) {
            Export export = new Export(this.saveFileDialog.FileName, false);
            export.Write(this.GetGraph());
            export.Close();
		}

        private const string paramTitle = "title";
        private const string defaultTitle = "";

		private const string defaultFileFilter = "Textov� soubory (*.txt)|*.txt|V�echny soubory (*.*)|*.*";
		private const string defaultFileExt = "txt";
		private const string defaultDirectory = "c:\\gcm";
	}
}