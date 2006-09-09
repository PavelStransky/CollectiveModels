using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
	/// <summary>
	/// Hlavní øídící formuláø
	/// </summary>
    public partial class MainForm: System.Windows.Forms.Form {
		//Kontexty
		private Expression.Context expContext;
		private Context controlContext;

		// Došlo k modifikaci?
		private bool modified = false;

        // Èíslo výsledkového formuláøe
        private static int resultNumber = 0;

		/// <summary>
		/// Nefunguje dobøe událost txtCommand_OnModifiedChanged, proto musíme vyøešit po svém
		/// </summary>
		private bool Modified {
			get {
				return this.modified;
			}
			set {
				if(this.modified != value) {
					this.modified = value;
					this.SetCaption();
				}
			}
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		public MainForm() {
			this.InitializeComponent();

			this.expContext = new Expression.Context();
			this.expContext.ExitRequest += new PavelStransky.Expression.Context.ExitEventHandler(context_ExitRequest);
			this.expContext.GraphRequest += new PavelStransky.Expression.Context.GraphRequestEventHandler(context_GraphRequest);

			// Kontext formuláøù
			this.controlContext = new Context();

			this.SetCaption();
		}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        public MainForm(string fileName) : this() {
            this.Open(fileName);
        }

		/// <summary>
		/// Událost z kontextu - žádost o uzavøení okna
		/// </summary>
		private void context_ExitRequest(object sender, EventArgs e) {
			this.Close();
		}

		#region Obsluha grafu
		private delegate void GraphRequestDelegate(object sender, GraphRequestEventArgs e);

		/// <summary>
		/// Událost z kontextu - žádost o vytvoøení grafu
		/// </summary>
		private void context_GraphRequest(object sender, GraphRequestEventArgs e) {
			// Spustíme ve vlastním threadu
			this.Invoke(new GraphRequestDelegate(this.GraphRequestInvoke), new object [] {sender, e});
		}

		/// <summary>
		/// Vytvoøení nového formuláøe musíme spustit ve vlastním threadu
		/// </summary>
		private void GraphRequestInvoke(object sender, GraphRequestEventArgs e) {
			GraphForm graphForm = this.controlContext.CreateForm(typeof(GraphForm), e.Variable.Name) as GraphForm;

			bool isGA = false;
			GraphArray ga = null;
			int count = 1;
			int lengthX = 1;
			int lengthY = 1;

			if(e.Variable.Item is GraphArray) {
				isGA = true;
				ga = e.Variable.Item as GraphArray;
				count = ga.Count;
				lengthX = ga.LengthX;
				lengthY = ga.LengthY;
			}

			if(graphForm.NumGraphControls() != count)
				graphForm.Controls.Clear();

			for(int i = 0; i < count; i++) {
				Expression.Graph g = null;
				int index = i;
				if(isGA) 
					g = ga[i];
				else {
					index = -1;
					g = e.Variable.Item as Expression.Graph;
				}
			
				RectangleF position = new RectangleF((float)(i % lengthX) / (float)lengthX, (int)(i / lengthX) / (float)lengthY, 1 / (float)lengthX, 1 / (float)lengthY);

				if(g is Expression.DensityGraph) {
					DensityGraph densityGraph = graphForm.GraphControl(index) as DensityGraph;

					if(densityGraph != null)
						densityGraph.SetVariable(e.Variable, index);
					else {
						graphForm.SuspendLayout();
						densityGraph = new DensityGraph(e.Variable, index, position);
						this.SetGraphStyles(densityGraph, graphForm, string.Format("{0}{1}", e.Variable.Name, i));
						graphForm.Controls.Add(densityGraph);
						graphForm.ResumeLayout();
					}
				}
				else if(g is Expression.LineGraph) {
					LineGraph lineGraph = graphForm.GraphControl(index) as LineGraph;

					if(lineGraph != null)
						lineGraph.SetVariable(e.Variable, index);
					else {
						graphForm.SuspendLayout();
						lineGraph = new LineGraph(e.Variable, index, position);
						this.SetGraphStyles(lineGraph, graphForm, string.Format("{0}{1}", e.Variable.Name, i));
						graphForm.Controls.Add(lineGraph);
						graphForm.ResumeLayout();
					}
				}
			}

			graphForm.Show();
		}

		/// <summary>
		/// Nastaví styly nového grafu na formuláøi
		/// </summary>
		/// <param name="graph">Graf</param>
		/// <param name="form">Formuláø</param>
		/// <param name="name">Jméno controlu</param>
		/// <param name="position">Umístìní controlu</param>
		private void SetGraphStyles(Graph graph, Form form, string name) {
			graph.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
			graph.SetPosition(form.Width, form.Height);
			graph.Name = name;
			graph.KeyDown += new KeyEventHandler(MainForm_KeyDown);
		}
		#endregion

		#region Otevírání a ukládání pøíkazù
		/// <summary>
		/// Menu Nový
		/// </summary>
		private void mnFileNew_Click(object sender, System.EventArgs e) {
			if(this.CheckForChanges())
				this.New();
		}

		/// <summary>
		/// Menu Otevøít
		/// </summary>
		private void mnFileOpen_Click(object sender, System.EventArgs e) {
			if(this.CheckForChanges()) {
                this.SetDialogProperties(this.openFileDialog);
				this.openFileDialog.ShowDialog();
			}
		}

		/// <summary>
		/// Menu Uložit
		/// </summary>
		private void mnFileSave_Click(object sender, System.EventArgs e) {
			this.Save();
		}

		/// <summary>
		/// Menu Uložit jako
		/// </summary>
		private void mnFileSaveAs_Click(object sender, System.EventArgs e) {
			this.SetDialogProperties(this.saveFileDialog);
			this.saveFileDialog.ShowDialog();
		}

		/// <summary>
		/// Otevøení historie
		/// </summary>
		private void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
			this.Directory = this.DirectoryFromFile(this.openFileDialog.FileName);

			DialogResult result = MessageBox.Show(this, messageOpen, captionOpen, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

			if (result != DialogResult.Cancel) {
				if(this.Open(this.openFileDialog.FileName) && result == DialogResult.Yes)
					this.txtCommand.ExecuteAll();
			}
		}

		/// <summary>
		/// Uložení historie
		/// </summary>
		private void saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
			this.Directory = this.DirectoryFromFile(this.saveFileDialog.FileName);

			this.Save(this.saveFileDialog.FileName);
		}

		/// <summary>
		/// Kontroluje, zda došlo ke zmìnám. Pokud ano, vyšle dotaz na uložení
		/// </summary>
		/// <returns>False, pokud je žádost o zrušení akce</returns>
		private bool CheckForChanges() {
			if(this.Modified) {
				DialogResult result = MessageBox.Show(this, 
					this.saveFileDialog.FileName == string.Empty ? messageChanged : string.Format(messageFileChanged, this.saveFileDialog.FileName),
					captionFileChanged, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

				if(result == DialogResult.Cancel)
					return false;
				else if(result == DialogResult.No)
					return true;
				else if(result == DialogResult.Yes)
					return this.Save();
			}

			return true;
		}

		/// <summary>
		/// Uloží soubor. Pokud nezná jméno, otevøe dialog
		/// </summary>
		/// <returns>False, pokud soubor nebyl uložen</returns>
		private bool Save() {
			if(this.saveFileDialog.FileName == string.Empty)
				switch(this.saveFileDialog.ShowDialog()) {
					case DialogResult.OK:
						return true;
					default:
						return false;
				}
			else {
				return this.Save(this.saveFileDialog.FileName);
			}
		}

		/// <summary>
		/// Uloží pøíkazy do souboru
		/// </summary>
		/// <param name="fileName">Jméno souboru</param>
		/// <returns>False, pokud se uložení nezdaøilo</returns>
		private bool Save(string fileName) {
			try {
				this.txtCommand.Export(fileName);
				
				this.modified = false;
				this.SetCaption();

				return true;
			}
			catch(DetailException e) {
				MessageBox.Show(this, string.Format(messageFailedSaveDetail, fileName, e.Message, e.DetailMessage), 
					captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return false;
			}
			catch(Exception e) {
				MessageBox.Show(this, string.Format(messageFailedSave, fileName, e.Message), 
					captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return false;
			}
		}

		/// <summary>
		/// Otevøe pøíkazy ze souboru
		/// </summary>
		/// <param name="fileName">Jméno souboru</param>
		/// <returns>False, pokud se otevøení nezdaøilo</returns>
		private bool Open(string fileName) {
			try {
				this.txtCommand.Import(fileName);

				this.saveFileDialog.FileName = fileName;

				this.modified = false;
				this.txtCommand.Modified = false;

				this.SetCaption();

				return true;
			}
			catch(DetailException e) {
				MessageBox.Show(this, string.Format(messageFailedOpenDetail, fileName, e.Message, e.DetailMessage), 
					captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return false;
			}
			catch(Exception e) {
				MessageBox.Show(this, string.Format(messageFailedOpen, fileName, e.Message), 
					captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return false;
			}
		}

		/// <summary>
		/// Nový
		/// </summary>
		private void New() {
            if(this.controlContext.CloseAll())
                return;

            this.expContext.Clear();

			this.txtCommand.Clear();
			this.txtCommand.Focus();

			this.saveFileDialog.FileName = string.Empty;
			this.modified = false;
			this.SetCaption();
		}

		/// <summary>
		/// Nastaví / vyzvedne aktuální adresáø
		/// </summary>
		private string Directory {
			get {
				if(this.expContext.Contains(directoryVariable) && this.expContext[directoryVariable].Item is string)
					return this.expContext[directoryVariable].Item as string;
				else
					return defaultDirectory;
			}
			set {
				this.expContext.SetVariable(directoryVariable, value);
			}
		}

		/// <summary>
		/// Podle názvu souboru urèí adresáø
		/// </summary>
		/// <param name="fileName">Název souboru s cestou</param>
		private string DirectoryFromFile(string fileName) {
			FileInfo f = new FileInfo(fileName);
			return f.DirectoryName;
		}
		#endregion

		/// <summary>
		/// Pøi žádosti o spuštìní pøíkazu
		/// </summary>
        private void txtCommand_ExecuteCommand(object sender, PavelStransky.Forms.ExecuteCommandEventArgs e) {
            string windowName = string.Format(defaultResultWindowName, resultNumber);

            // Chceme nové okno
            if(e.NewWindow)
                if(this.controlContext.Contains(windowName))
                    windowName = string.Format(defaultResultWindowName, ++resultNumber);

            ResultForm f = this.controlContext.CreateForm(typeof(ResultForm), windowName) as ResultForm;
            if(f.Calulating) {
                MessageBox.Show(this, messageCalculationRunning);
            }
            else {
                f.SetExpression(this.expContext, e.Expression);
                f.Show();
                f.Start();
            }
        }

		/// <summary>
		/// Pøi zmìnì textu zmìníme atribut Modified
		/// </summary>
		private void txtCommand_TextChanged(object sender, System.EventArgs e) {
			this.Modified = txtCommand.Modified;
		}

		/// <summary>
		/// nastaví nadpis okna
		/// </summary>
		private void SetCaption() {
			this.Text = string.Format(this.saveFileDialog.FileName == string.Empty ? titleFormat : titleFormatFile, 
				defaultName, this.saveFileDialog.FileName, this.Modified ? asterisk : string.Empty);
		}

		/// <summary>
		/// Nastaví vlastnosti dialogu
		/// </summary>
		/// <param name="dialog">Dialog</param>
		private void SetDialogProperties(FileDialog dialog) {
			dialog.Reset();
			dialog.Filter = defaultFileFilter;
			dialog.DefaultExt = defaultFileExt;
			dialog.InitialDirectory = this.Directory;
		}

		/// <summary>
		/// Pøi aktivaci okna nastavíme focus
		/// </summary>
		private void MainForm_Activated(object sender, System.EventArgs e) {
			this.txtCommand.Focus();
		}

		/// <summary>
		/// Pro KeyDown na kontextu
		/// </summary>
		private void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.Alt || e.Shift)
				return;
			if(e.Control && e.KeyValue <= 'Z' && e.KeyValue >= 'A') {
				try {
					e.Handled = this.expContext.HotKey((char)e.KeyValue);
				}
				catch(DetailException exc) {
					MessageBox.Show(this, string.Format("{0}\n\n{1}", exc.Message, exc.DetailMessage));
				}
				catch(Exception exc) {
					MessageBox.Show(this, string.Format("{0}", exc.Message));
				}
			}
		}

        /// <summary>
        /// Pøi uzavírání okna musíme uzavøít všechny pøidružené formuláøe
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
			e.Cancel = !this.CheckForChanges();
            if(!e.Cancel) 
                e.Cancel = this.controlContext.CloseAll();
        }

        private const string defaultFileFilter = "Soubory historie (*.gcm)|*.gcm|Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";
		private const string defaultFileExt = "his";
		private const string defaultDirectory = "c:\\gcm";
		private const string directoryVariable = "_dir";

		private const string messageOpen = "Chcete, aby byly všechny pøíkazy historie po otevøení automaticky spuštìny?";
		private const string captionOpen = "Otevøení historie";

		private const string messageFileChanged = "Soubor '{0}' byl zmìnìn. Chcete zmìny uložit?";
		private const string messageChanged = "Data nejsou uložena. Chcete je uložit?";
		private const string captionFileChanged = "Uložení souboru";

		private const string messageFailedSave = "Uložení do souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}";
		private const string messageFailedSaveDetail = "Uložení do souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}\n\n{2}";
		private const string captionFailedSave = "Chyba!";
		private const string messageFailedOpen = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}";
		private const string messageFailedOpenDetail = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}\n\n{2}";
		private const string captionFailedOpen = "Chyba!";

        private const string messageCalculationRunning = "V aktuálním oknì probíhá výpoèet, nelze spustit nový výpoèet!";
        private const string defaultResultWindowName = "Result{0}";

		private const string defaultName = "GCM Prográmek";
		private const string titleFormatFile = "{0} - {1} {2}";
		private const string titleFormat = "{0} {2}";
		private const string asterisk = "*";
	}
}
