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
    /// Formuláø pro editaci
    /// </summary>
    public partial class Editor : Form, IExportable {
        //Kontext s výrazy
        private Context context;

        // Jméno souboru
        private string fileName = string.Empty;

        // Došlo k modifikaci?
        private bool modified = false;

        // Èíslo výsledkového formuláøe
        private static int resultNumber = 0;

        /// <summary>
        /// Kontext
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Dialog Uložit
        /// </summary>
        public SaveFileDialog SaveFileDialog { get { return this.saveFileDialog; } }

        /// <summary>
        /// Nefunguje dobøe událost txtCommand_OnModifiedChanged, proto musíme vyøešit po svém
        /// </summary>
        public bool Modified {
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
        /// Pøeète / nastaví jméno souboru
        /// </summary>
        public string FileName {
            get {
                return this.fileName;
            }
            set {
                this.Directory = this.DirectoryFromFile(value);
                this.fileName = value;
                this.SetCaption();
            }
        }

        /// <summary>
        /// Nastaví / vyzvedne aktuální adresáø
        /// </summary>
        public string Directory {
            get {
                if(this.context.Contains(directoryVariable) && this.context[directoryVariable].Item is string)
                    return this.context[directoryVariable].Item as string;
                else
                    return string.Empty;
            }
            set {
                this.context.SetVariable(directoryVariable, value);
                this.saveFileDialog.InitialDirectory = value;
            }
        }

        /// <summary>
        /// Inicializuje eventy pro context
        /// </summary>
        private void InitializeEvents() {
            this.context.ExitRequest += new PavelStransky.Expression.Context.ExitEventHandler(context_ExitRequest);
            this.context.GraphRequest += new PavelStransky.Expression.Context.GraphRequestEventHandler(context_GraphRequest);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Editor() {
            this.InitializeComponent();
            this.context = new Expression.Context();
            this.InitializeEvents();
            this.SetCaption();
        }

        /// <summary>
        /// Událost z kontextu - žádost o uzavøení okna
        /// </summary>
        private void context_ExitRequest(object sender, EventArgs e) {
            this.Close();
        }

        #region Obsluha grafu
        private delegate void GraphRequestDelegate(Variable v);

        /// <summary>
        /// Událost z kontextu - žádost o vytvoøení grafu
        /// </summary>
        private void context_GraphRequest(object sender, GraphRequestEventArgs e) {
            // Spustíme ve vlastním threadu
            this.Invoke(new GraphRequestDelegate(this.CreateGraph), new object[] { e.Variable });
        }

        /// <summary>
        /// Vytvoøení nového formuláøe musíme spustit ve vlastním threadu
        /// </summary>
        private void CreateGraph(Variable v) {
            GraphForm graphForm = this.NewParentForm(typeof(GraphForm), v.Name) as GraphForm;

            bool isGA = false;
            GraphArray ga = null;
            int count = 1;
            int lengthX = 1;
            int lengthY = 1;

            if(v.Item is GraphArray) {
                isGA = true;
                ga = v.Item as GraphArray;
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
                    g = v.Item as Expression.Graph;
                }

                RectangleF position = new RectangleF((float)(i % lengthX) / (float)lengthX, (int)(i / lengthX) / (float)lengthY, 1 / (float)lengthX, 1 / (float)lengthY);

                if(g is Expression.DensityGraph) {
                    DensityGraph densityGraph = graphForm.GraphControl(index) as DensityGraph;

                    if(densityGraph != null)
                        densityGraph.SetVariable(v, index);
                    else {
                        graphForm.SuspendLayout();
                        densityGraph = new DensityGraph(v, index, position);
                        this.SetGraphStyles(densityGraph, graphForm, string.Format("{0}{1}", v.Name, i));
                        graphForm.Controls.Add(densityGraph);
                        graphForm.ResumeLayout();
                    }
                }
                else if(g is Expression.LineGraph) {
                    LineGraph lineGraph = graphForm.GraphControl(index) as LineGraph;

                    if(lineGraph != null)
                        lineGraph.SetVariable(v, index);
                    else {
                        graphForm.SuspendLayout();
                        lineGraph = new LineGraph(v, index, position);
                        this.SetGraphStyles(lineGraph, graphForm, string.Format("{0}{1}", v.Name, i));
                        graphForm.Controls.Add(lineGraph);
                        graphForm.ResumeLayout();
                    }
                }
            }

            graphForm.Show();
            this.Activate();
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
            graph.KeyDown += new KeyEventHandler(Editor_KeyDown);
        }
        #endregion

        /// <summary>
        /// Vytvoøí nový podøízený formuláø
        /// </summary>
        /// <param name="type">Typ formuláøe</param>
        /// <param name="name">Název okna</param>
        public ChildForm NewParentForm(Type type, string name) {
            ChildForm result;

            Form[] forms = this.MdiParent.MdiChildren;
            for(int i = 0; i < forms.Length; i++) {
                result = forms[i] as ChildForm;

                if(result != null && result.ParentEditor == this && result.Name == name && result.GetType() == type)
                    return result;
            }

            if(type == typeof(GraphForm)) {
                result = new GraphForm();
                result.Location = new Point(margin, this.MdiParent.Height - result.Size.Height - 8 * margin);
            }
            else if(type == typeof(ResultForm)) {
                result = new ResultForm();
                result.Location = new Point(this.MdiParent.Width - result.Size.Width - 2 * margin, margin);
            }
            else
                result = new ChildForm();

            result.Name = name;
            result.Text = name;
            result.ParentEditor = this;
            result.MdiParent = this.MdiParent;

            return result;
        }

        #region Otevírání a ukládání pøíkazù
        /// <summary>
        /// Pokusí se zavøít všechna podøízená okna
        /// </summary>
        /// <returns>True, pokud je vše OK a uzavøení se podaøilo</returns>
        private bool CloseChildWindow() {
            MainForm form = this.MdiParent as MainForm;

            for(int i = 0; i < form.MdiChildren.Length; i++) {
                // Všechny podøízené formuláøe
                ChildForm childForm = form.MdiChildren[i] as ChildForm;

                if(childForm != null && childForm.ParentEditor == this) {
                    ResultForm resultForm = childForm as ResultForm;
                    if(resultForm != null && resultForm.Calulating) {
                        DialogResult result = MessageBox.Show(this,
                            string.Format(messageClose, resultForm.Name),
                            captionClose, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        switch(result) {
                            case DialogResult.Yes:
                                resultForm.Abort(true);
                                break;
                            case DialogResult.No:
                                return false;
                        }
                    }

                    childForm.Close();

                    // Vymazali jsme formuláø, proto musíme vrátit index zpìt
                    i--;
                }
            }

            return true;
        }

        /// <summary>
        /// Kontroluje, zda došlo ke zmìnám. Pokud ano, vyšle dotaz na uložení
        /// </summary>
        /// <returns>False, pokud je žádost o zrušení akce</returns>
        private bool CheckForChanges() {
            if(this.Modified) {
                DialogResult result = MessageBox.Show(this,
                    this.fileName == string.Empty ? messageChanged : string.Format(messageFileChanged, this.fileName),
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
        /// Uloží pøíkazy; pokud zatím neznáme jméno souboru, ukáže dialog
        /// </summary>
        /// <returns></returns>
        public bool Save() {
            if(this.fileName == null || this.fileName == string.Empty) {
                if(this.saveFileDialog.ShowDialog() == DialogResult.OK)
                    return this.Save(this.saveFileDialog.FileName);
                else
                    return false;
            }
            else
                return this.Save(this.fileName);
        }

        /// <summary>
        /// Uloží pøíkazy a vždy zobrazí dialog
        /// </summary>
        public bool SaveAs() {
            this.saveFileDialog.FileName = this.fileName;

            if(this.saveFileDialog.ShowDialog() == DialogResult.OK)
                return this.Save(this.saveFileDialog.FileName);
            else
                return false;
        }

        /// <summary>
        /// Uloží pøíkazy do souboru
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        /// <returns>False, pokud se uložení nezdaøilo</returns>
        public bool Save(string fileName) {
            this.FileName = fileName;
            Export export = null;

            try {
                export = new Export(fileName, true);
                export.Write(this);

                Form parentForm = this.MdiParent;

                int num = 0;
                for(int i = 0; i < parentForm.MdiChildren.Length; i++) {
                    ChildForm childForm = parentForm.MdiChildren[i] as ChildForm;
                    if(childForm as IExportable != null && childForm.ParentEditor == this)
                        num++;
                }

                export.B.Write(num);
                for(int i = 0; i < parentForm.MdiChildren.Length; i++) {
                    ChildForm childForm = parentForm.MdiChildren[i] as ChildForm;
                    if(childForm as IExportable != null && childForm.ParentEditor == this)
                        export.Write(childForm);
                }
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
            finally {
                export.Close();
            }

            this.Modified = false;
            this.SetCaption();

            return true;
        }
        #endregion

        /// <summary>
        /// Pøi žádosti o spuštìní pøíkazu
        /// </summary>
        private void txtCommand_ExecuteCommand(object sender, PavelStransky.Forms.ExecuteCommandEventArgs e) {
            // Chceme nové okno
            if(e.NewWindow)
                resultNumber++;

            string windowName = string.Format(defaultResultWindowName, resultNumber);

            ResultForm f = this.NewParentForm(typeof(ResultForm), windowName) as ResultForm;

            if(f.Calulating) {
                f.Activate();
                MessageBox.Show(this, messageCalculationRunning);
                this.Activate();
            }
            else {
                f.SetExpression(this.context, e.Expression);
                f.Show();
                f.Start();

                this.Activate();
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
            this.Text = string.Format(this.fileName == string.Empty ? titleFormat : titleFormatFile,
                defaultName, this.fileName, this.Modified ? asterisk : string.Empty);
        }

        /// <summary>
        /// Pøi aktivaci okna nastavíme focus
        /// </summary>
        private void Editor_Activated(object sender, System.EventArgs e) {
            this.txtCommand.Focus();
        }

        /// <summary>
        /// Pro KeyDown na kontextu
        /// </summary>
        private void Editor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            if(e.Alt || e.Shift)
                return;
            if(e.Control && e.KeyValue <= 'Z' && e.KeyValue >= 'A') {
                try {
                    e.Handled = this.context.HotKey((char)e.KeyValue);
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
            e.Cancel = !(this.CheckForChanges() && this.CloseChildWindow());
        }

        /// <summary>
        /// Podle názvu souboru urèí adresáø
        /// </summary>
        /// <param name="fileName">Název souboru s cestou</param>
        private string DirectoryFromFile(string fileName) {
            FileInfo f = new FileInfo(fileName);
            return f.DirectoryName;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah kontextu do souboru
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

            b.Write(this.txtCommand.Rtf);
            b.Write(this.txtCommand.SelectionStart);

            export.Write(this.context);
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

            this.txtCommand.Rtf = b.ReadString();
            this.txtCommand.SelectionStart = b.ReadInt32();

            this.context = import.Read() as Context;
            this.InitializeEvents();
        }
        #endregion

        private const string directoryVariable = "_dir";

        private const string messageOpen = "Chcete, aby byly všechny pøíkazy historie po otevøení automaticky spuštìny?";
        private const string captionOpen = "Otevøení historie";

        private const string messageFileChanged = "Soubor '{0}' byl zmìnìn. Chcete zmìny uložit?";
        private const string messageChanged = "Data nejsou uložena. Chcete je uložit?";
        private const string captionFileChanged = "Uložení souboru";

        private const string messageFailedSave = "Uložení do souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}";
        private const string messageFailedSaveDetail = "Uložení do souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedSave = "Chyba!";

        private const string messageClose = "V oknì {0} probíhá výpoèet. Opravdu chcete okno uzavøít a výpoèet ukonèit?";
        private const string captionClose = "Varování";

        private const string messageCalculationRunning = "V aktuálním oknì probíhá výpoèet, nelze spustit nový výpoèet!";
        private const string defaultResultWindowName = "Result{0}";

        private const string defaultName = "Utitled";
        private const string titleFormatFile = "{1} {2}";
        private const string titleFormat = "{0} {2}";
        private const string asterisk = "*";

        private const int margin = 8;
    }
}
