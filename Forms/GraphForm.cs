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
	/// Prázdný formuláø. Grafy do nìj pøidáváme my
	/// </summary>
	public partial class GraphForm : ChildForm, IExportable {
        private GraphControl[] graphControl;

        // Fronta procesù na pozadí
        private ArrayList backgroundWorkers = new ArrayList();
        // Probíhající proces na pozadí - musíme ukonèit pøed uzavøením okna
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// Poèet sloucpù
        /// </summary>
        private int numColumns = 1;

		/// <summary>
		/// Konstruktor
		/// </summary>
        public GraphForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Nastaví data do grafu
        /// </summary>
        /// <param name="graphs">Objekt s daty</param>
        /// <param name="numColumns">Poèet sloupcù</param>
        public void SetGraph(Expression.Array graphs, int numColumns) {
            int count = graphs.Count;
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
        /// Zobrazí progressBar a spustí proces; pokud již nìjaký proces bìží, tento uloží do fronty
        /// </summary>
        /// <param name="lblText">Text</param>
        /// <param name="backgroundWorker">Proces</param>
        /// <param name="argument">Vstupní parametr procesu</param>
        public void NewProcess(string lblText, BackgroundWorker backgroundWorker, object argument) {
            // Nìjaký proces už probíhá - musíme poèkat
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
        /// Zobrazí progressBar a spustí proces; pokud již nìjaký proces bìží, tento uloží do fronty
        /// </summary>
        /// <param name="lblText">Text</param>
        /// <param name="backgroundWorker">Proces</param>
        public void NewProcess(string lblText, BackgroundWorker backgroundWorker) {
            this.NewProcess(lblText, backgroundWorker, null);
        }

        /// <summary>
        /// Spustí proces
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

            // Není žádný èekající proces
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
        /// Zavírání formuláøe - musíme pøerušit proces na pozadí
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            if(this.backgroundWorker != null && this.backgroundWorker.IsBusy) 
                this.backgroundWorker.CancelAsync();
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
            b.Write(this.numColumns);

            int length = this.graphControl.Length;
            b.Write(length);
            for(int i = 0; i < length; i++)
                export.Write(this.graphControl[i].GetGraph());
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
            int numColumns = b.ReadInt32();
            int length = b.ReadInt32();

            Expression.Array graphs = new Expression.Array();
            for(int i = 0; i < length; i++)
                graphs.Add(import.Read());
            this.SetGraph(graphs, numColumns);
        }
        #endregion

        // Okraj okolo GraphControlu v pixelech
        private int margin = 8;
    }
}
