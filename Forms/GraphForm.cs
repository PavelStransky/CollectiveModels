using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Pr�zdn� formul��. Grafy do n�j p�id�v�me my
	/// </summary>
	public partial class GraphForm : ChildForm, IExportable {
        private GraphControl[] graphControl;

        // Fronta proces� na pozad�
        private ArrayList backgroundWorkers = new ArrayList();
        // Prob�haj�c� proces na pozad� - mus�me ukon�it p�ed uzav�en�m okna
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// Po�et sloucp� a ��dek
        /// </summary>
        private int numColumns = 1;

		/// <summary>
		/// Konstruktor
		/// </summary>
        public GraphForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Nastav� data do grafu
        /// </summary>
        /// <param name="graphs">Objekt s daty</param>
        /// <param name="numColumns">Po�et sloupc�</param>
        public void SetGraph(TArray graphs, int numColumns) {
            int count = graphs.Length;
            int numRows = (count - 1) / numColumns + 1;

            Rectangle r = this.ClientRectangle;
            r.Height -= this.progress.Height;

            int xStep = (r.Width - margin) / numColumns;
            int yStep = (r.Height - margin) / numRows;

            this.graphControl = new GraphControl[count];
            this.SuspendLayout();
            this.Controls.Clear();

            this.progress.Visible = false;
            this.lblProgress.Visible = false;
            this.lblRestProcess.Visible = false;
            this.Controls.Add(this.progress);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblRestProcess);

            for(int i = 0; i < count; i++) {
                GraphControl gc = new GraphControl();
                gc.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                gc.Location = new Point(r.X + margin + xStep * (i % numColumns), r.Y + margin + yStep * (i / numColumns));
                gc.Size = new Size(xStep - margin, yStep - margin);
                this.Controls.Add(gc);
                this.graphControl[i] = gc;
                gc.SetGraph(graphs[i] as Graph);
            }

            this.ResumeLayout();

            this.numColumns = numColumns;
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);

            if(this.graphControl == null)
                return;

            int length = this.graphControl.Length;
            int numRows = (length - 1) / numColumns + 1;

            Rectangle r = this.ClientRectangle;
            r.Height -= this.progress.Height;

            int xStep = (r.Width - margin) / numColumns;
            int yStep = (r.Height - margin) / numRows;

            this.SuspendLayout();
            for(int i = 0; i < length; i++) {
                GraphControl gc = this.graphControl[i];
                gc.Location = new Point(r.X + margin + xStep * (i % numColumns), r.Y + margin + yStep * (i / numColumns));
                gc.Size = new Size(xStep - margin, yStep - margin);
            }
            this.ResumeLayout();
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

            int length = this.graphControl.Length;
            param.Add(length);
            for(int i = 0; i < length; i++)
                param.Add(this.graphControl[i].GetGraph(), string.Format("GraphData{0}", i));

            param.Add(this.WindowState.ToString(), "WindowState");

            param.Export(export);
        }

        /// <summary>
        /// Na�te obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            IEParam param = new IEParam(import);

            this.Location = new Point((int)param.Get(0), (int)param.Get(0));
            this.Size = new Size((int)param.Get(this.Size.Width), (int)param.Get(this.Size.Height));

            this.Name = (string)param.Get(string.Empty);
            this.Text = this.Name;
            this.numColumns = (int)param.Get(1);

            int length = (int)param.Get(0);
            TArray graphs = new TArray(typeof(Graph), length);
            for(int i = 0; i < length; i++)
                graphs[i] = param.Get();

            this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), (string)param.Get(FormWindowState.Normal.ToString()), true);

            this.SetGraph(graphs, numColumns);
        }
        #endregion

        // Okraj okolo GraphControlu v pixelech
        private int margin = 8;
    }
}
