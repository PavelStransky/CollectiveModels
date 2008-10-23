using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using PavelStransky.Core;
using PavelStransky.Expression;

using PavelStransky.Math;

namespace PavelStransky.Forms {
	/// <summary>
	/// Pr�zdn� formul��. Grafy do n�j p�id�v�me my
	/// </summary>
	public partial class SingleGraphForm : ChildForm, IExportable {
        // Fronta proces� na pozad�
        private ArrayList backgroundWorkers = new ArrayList();
        // Prob�haj�c� proces na pozad� - mus�me ukon�it p�ed uzav�en�m okna
        private BackgroundWorker backgroundWorker;

        // Po�et sloupc�
        private int numColumns;

        // Data
        private TArray graphs;

        // Skute�n� ���ka a v��ka podle zad�n� (kv�li ukl�d�n�, velikost okna m��e b�t toti� nejv��e rovna velikosti pracovn� plochy)
        private int realWidth = 0;
        private int realHeight = 0;

        /// <summary>
        /// Skute�n� ���ka okna
        /// </summary>
        public int RealWidth { get { return this.realWidth; } set { this.realWidth = value; } }

        /// <summary>
        /// Skute�n� v��ka okna
        /// </summary>
        public int RealHeight { get { return this.realHeight; } set { this.realHeight = value; } }

		/// <summary>
		/// Konstruktor
		/// </summary>
        public SingleGraphForm() {
            this.InitializeComponent();
            this.CreateWorkers();

            this.sfdText.Filter = WinMain.FileFilterTxt;
            this.sfdText.DefaultExt = WinMain.FileExtTxt;
            this.sfdAnim.Filter = WinMain.FileFilterGif;
            this.sfdAnim.DefaultExt = WinMain.FileExtGif;
            this.sfdSeq.Filter = WinMain.FileFilterPicture;
            this.sfdSeq.DefaultExt = WinMain.FileExtPicture;
        }

        /// <summary>
        /// Nastav� data do grafu
        /// </summary>
        /// <param name="graphs">Objekt s daty</param>
        /// <param name="numColumns">Po�et sloupc�</param>
        public void SetGraph(TArray graphs, int numColumns) {
            this.graphs = graphs;

            int count = graphs.Length;
            int numRows = (count - 1) / numColumns + 1;

            double xStep = 1.0 / numColumns;
            double yStep = 1.0 / numRows;

            for(int i = 0; i < count; i++)
                (graphs[i] as Graph).SetRelativeWindow(xStep * (i % numColumns), yStep * (i / numColumns), xStep, yStep);

            this.graphicsBox.SetGraphs(graphs);
            this.numColumns = numColumns;
        }

        /// <summary>
        /// Zobraz� progressBar a spust� proces; pokud ji� n�jak� proces b��, tento ulo�� do fronty
        /// </summary>
        /// <param name="lblText">Text</param>
        /// <param name="backgroundWorker">Proces</param>
        /// <param name="argument">Vstupn� parametr procesu</param>
        public void NewProcess(string lblText, BackgroundWorker backgroundWorker, object argument) {
            // N�jak� proces u� prob�h� - mus�me po�kat
            if(this.backgroundWorker != null && this.backgroundWorker.IsBusy) {
                this.backgroundWorkers.Add(lblText);
                this.backgroundWorkers.Add(backgroundWorker);
                this.backgroundWorkers.Add(argument);
                this.lblRestProcess.Text = (this.backgroundWorkers.Count / 3).ToString();
            }
            else {
                this.SuspendLayout();
                this.lblProgress.Text = lblText;
                this.lblProgress.Visible = true;
                this.progress.Visible = true;
                this.lblRestProcess.Text = "0";
                this.lblRestProcess.Visible = true;
                this.ResumeLayout();

                this.StartProcess(backgroundWorker, argument);
            }
        }

        /// <summary>
        /// Zobraz� progressBar a spust� proces; pokud ji� n�jak� proces b��, tento ulo�� do fronty
        /// </summary>
        /// <param name="lblText">Text</param>
        /// <param name="backgroundWorker">Proces</param>
        public void NewProcess(string lblText, BackgroundWorker backgroundWorker) {
            this.NewProcess(lblText, backgroundWorker, null);
        }

        /// <summary>
        /// Spust� proces
        /// </summary>
        /// <param name="backgroundWorker">Proces</param>
        /// <param name="argument">Parametr procesu</param>
        private void StartProcess(BackgroundWorker backgroundWorker, object argument) {
            this.backgroundWorker = backgroundWorker;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            this.backgroundWorker.RunWorkerAsync(argument);
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if(this.backgroundWorker.CancellationPending)
                return;

            if(e.UserState is string)
                this.lblProgress.Text = e.UserState as string;

            this.progress.Value = e.ProgressPercentage;
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if(this.backgroundWorker.CancellationPending || e.Cancelled)
                return;

            // Nen� ��dn� �ekaj�c� proces
            if(this.backgroundWorkers.Count == 0) {
                this.lblProgress.Visible = false;
                this.progress.Visible = false;
                this.lblRestProcess.Visible = false;
            }
            else {
                string lblText = this.backgroundWorkers[0] as string;
                BackgroundWorker backgroundWorker = this.backgroundWorkers[1] as BackgroundWorker;
                object argument = this.backgroundWorkers[2];
                this.backgroundWorkers.RemoveRange(0, 3);

                this.lblProgress.Text = lblText;
                this.lblRestProcess.Text = (this.backgroundWorkers.Count / 3).ToString();

                this.StartProcess(backgroundWorker, argument);
            }
        }

        /// <summary>
        /// Zav�r�n� formul��e - mus�me p�eru�it proces na pozad�
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            if(this.backgroundWorker != null && this.backgroundWorker.IsBusy) 
                this.backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// P�i pohybu krysou nastav�me toolTip
        /// </summary>
        private void graphicsBox_MouseMove(object sender, MouseEventArgs e) {
            this.toolTip.SetToolTip(this.graphicsBox, this.graphicsBox.ToolTip(e.X, e.Y));
        }

        /// <summary>
        /// Zah�jen� Drag & Drop akce
        /// a zaznamen�n� aktu�ln�ho grafu (pro ukl�d�n�)
        /// </summary>
        private void graphicsBox_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left && e.Clicks == 1) {
                object o = this.graphicsBox.CoordintatesFromPosition(e.X, e.Y);
                if(o != null)
                    this.DoDragDrop(o, DragDropEffects.Copy);
            }

            else if(e.Button == MouseButtons.Right) {
                this.activeGraph = this.graphicsBox.ActiveGraph(e.X, e.Y);
            }
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah formul��e do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.Location.X, "X");
            param.Add(this.Location.Y, "Y");
            param.Add(this.Size.Width, "Width");
            param.Add(this.Size.Height, "Height");

            param.Add(this.Name, "GraphName");
            param.Add(this.numColumns, "NumColumns");

            param.Add(this.graphs);
            param.Add(this.WindowState.ToString(), "WindowState");

            param.Export(export);
        }

        /// <summary>
        /// Na�te obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public SingleGraphForm(Core.Import import)
            : this() {
            IEParam param = new IEParam(import);

            this.Location = new Point((int)param.Get(0), (int)param.Get(0));
            this.Size = new Size((int)param.Get(this.Size.Width), (int)param.Get(this.Size.Height));

            this.Name = (string)param.Get(string.Empty);
            this.Text = this.Name;
            this.numColumns = (int)param.Get(1);

            if(import.VersionNumber > 5) {
                this.graphs = (TArray)param.Get();
                this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), (string)param.Get(FormWindowState.Normal.ToString()), true);
            }

            if(this.graphs != null)
                this.SetGraph(graphs, numColumns);
        }
        #endregion

        private void cmnStopAnimation_Click(object sender, EventArgs e) {
            GraphItem[] graphItems = this.graphicsBox.GraphItems;
            int count = graphItems.Length;

            for(int i = 0; i < count; i++)
                graphItems[i].PauseTimer();
        }

        private void cmnStartAnimation_Click(object sender, EventArgs e) {
            GraphItem[] graphItems = this.graphicsBox.GraphItems;
            int count = graphItems.Length;

            for(int i = 0; i < count; i++)
                graphItems[i].StartTimer();
        }
    }
}
