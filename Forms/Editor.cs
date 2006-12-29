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
        private int resultNumber = 0;

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
        /// <param name="context">Kontext</param>
        private void InitializeEvents(Context context) {
            context.ExitRequest += new Context.ExitEventHandler(context_ExitRequest);
            context.GraphRequest += new Context.GraphRequestEventHandler(context_GraphRequest);
            context.NewContextRequest += new Context.ContextEventHandler(context_NewContextRequest);
            context.SetContextRequest += new Context.ContextEventHandler(context_SetContextRequest);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Editor() {
            this.InitializeComponent();
            this.context = new Context();
            this.InitializeEvents(this.context);
            this.SetCaption();
        }

        /// <summary>
        /// Událost z kontextu - žádost o uzavøení okna
        /// </summary>
        private void context_ExitRequest(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// Událost z kontextu - zmìna kontextu
        /// </summary>
        private void context_SetContextRequest(object sender, ContextEventArgs e) {
            this.context = e.Context;
        }

        /// <summary>
        /// Událost z kontextu - nový kontext (musíme nastavit eventy)
        /// </summary>
        void context_NewContextRequest(object sender, ContextEventArgs e) {
            this.InitializeEvents(e.Context);
        }

        #region Obsluha grafu
        private delegate void GraphRequestDelegate(GraphRequestEventArgs e);

        /// <summary>
        /// Událost z kontextu - žádost o vytvoøení grafu
        /// </summary>
        private void context_GraphRequest(object sender, GraphRequestEventArgs e) {
            // Spustíme ve vlastním threadu
            this.Invoke(new GraphRequestDelegate(this.CreateGraph), new object[] { e });
        }

        /// <summary>
        /// Vytvoøení nového formuláøe musíme spustit ve vlastním threadu
        /// </summary>
        private void CreateGraph(GraphRequestEventArgs e) {
            GraphForm graphForm = this.NewParentForm(typeof(GraphForm), e.Name) as GraphForm;
            graphForm.SetGraph(e.Graphs, e.NumColumns);

            graphForm.Show();
            this.Activate();
        }

        #endregion

        /// <summary>
        /// Nastaví eventy všech ResultForms (je nutné po otevøení ze souboru)
        /// </summary>
        public void SetResultFormsEvents() {
            Form[] forms = this.MdiParent.MdiChildren;
            for(int i = 0; i < forms.Length; i++) {
                ResultForm rf = forms[i] as ResultForm;

                if(rf != null && rf.ParentEditor == this) {
                    rf.CalcStarted += new EventHandler(this.Editor_CalcStarted);
                    rf.CalcFinished += new EventHandler(this.Editor_CalcFinished);
                    rf.CalcPaused += new EventHandler(this.Editor_CalcPaused);
                    rf.FormClosed += new FormClosedEventHandler(this.result_FormClosed);
                    this.mrbResult.Add(rf.Name, rf.Text);
                }
            }
        }

        /// <summary>
        /// Pokusí se nalézt formuláø s daným názvem a typem
        /// </summary>
        /// <param name="type">Typ formuláøe</param>
        /// <param name="name">Název okna</param>
        private ChildForm GetChildFormFromName(Type type, string name) {
            ChildForm result;

            Form[] forms = this.MdiParent.MdiChildren;
            for(int i = 0; i < forms.Length; i++) {
                result = forms[i] as ChildForm;

                if(result != null && result.ParentEditor == this && result.Name == name && result.GetType() == type)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Vytvoøí nový podøízený formuláø
        /// </summary>
        /// <param name="type">Typ formuláøe</param>
        /// <param name="name">Název okna</param>
        public ChildForm NewParentForm(Type type, string name) {
            ChildForm result = this.GetChildFormFromName(type, name);

            if(result != null)
                return result;

            if(type == typeof(GraphForm)) {
                result = new GraphForm();
                result.Location = new Point(margin, this.MdiParent.Height - result.Size.Height - 8 * margin);
            }
            else if(type == typeof(ResultForm)) {
                result = new ResultForm();
                result.Location = new Point(this.MdiParent.Width - result.Size.Width - 2 * margin, margin);
                (result as ResultForm).CalcStarted += new EventHandler(this.Editor_CalcStarted);
                (result as ResultForm).CalcFinished += new EventHandler(this.Editor_CalcFinished);
                (result as ResultForm).CalcPaused += new EventHandler(this.Editor_CalcPaused);
                result.FormClosed += new FormClosedEventHandler(this.result_FormClosed);
                this.mrbResult.Add(name, name);
            }
            else
                result = new ChildForm();

            result.Name = name;
            result.Text = name;
            result.ParentEditor = this;
            result.MdiParent = this.MdiParent;           

            return result;
        }

        /// <summary>
        /// Zavøení formuláøe s výsledky
        /// </summary>
        private void result_FormClosed(object sender, FormClosedEventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null) 
                this.mrbResult.Remove(r.Name);
        }

        /// <summary>
        /// Zaèátek výpoètu
        /// </summary>
        private void Editor_CalcStarted(object sender, EventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null) 
                this.mrbResult.SetBackColor(r.Name, Color.Red);
        }

        /// <summary>
        /// Konec výpoètu
        /// </summary>
        private void Editor_CalcFinished(object sender, EventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null)
                this.mrbResult.SetDefaultBackColor(r.Name);
        }

        /// <summary>
        /// Pøerušený výpoèet
        /// </summary>
        private void Editor_CalcPaused(object sender, EventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null)
                this.mrbResult.SetBackColor(r.Name, Color.Blue);
        }

        /// <summary>
        /// Kliknutí na RadioButton
        /// </summary>
        private void mrbResult_RBClick(object sender, MultipleRadioButtonEventArgs e) {
            ResultForm result = this.GetChildFormFromName(typeof(ResultForm), e.Name) as ResultForm;

            if(result != null) {
                result.Activate();
                this.Activate();
            }
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
            string fileNameSave = fileName + ".sav";
            Export export = null;

            try {
                export = new Export(fileNameSave, true);
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

                export.Close();

                // Doèasný soubor, do kterého jsme ukládali, pøejmenujeme na orig. verzi
                if(File.Exists(fileName))
                    File.Delete(fileName);
                File.Move(fileNameSave, fileName);

                this.Modified = false;            
                this.SetCaption();
            }
            catch(DetailException e) {
                export.Close();
                
                // Doèasný soubor, do kterého jsme ukládali, smažeme
                if(File.Exists(fileNameSave))
                    File.Delete(fileNameSave);

                MessageBox.Show(this, string.Format(messageFailedSaveDetail, fileName, e.Message, e.DetailMessage),
                    captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch(Exception e) {
                export.Close();

                // Doèasný soubor, do kterého jsme ukládali, smažeme
                if(File.Exists(fileNameSave))
                    File.Delete(fileNameSave);

                MessageBox.Show(this, string.Format(messageFailedSave, fileName, e.Message),
                    captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Pøi žádosti o spuštìní pøíkazu
        /// </summary>
        private void txtCommand_ExecuteCommand(object sender, PavelStransky.Forms.ExecuteCommandEventArgs e) {
            string windowName = null;
            if(!e.NewWindow && !this.mrbResult.IsNewChecked)
                windowName = this.mrbResult.GetCheckedName();

            // Chceme nové okno
            if(windowName == null)
                windowName = string.Format(defaultResultWindowName, ++this.resultNumber);

            ResultForm f = this.NewParentForm(typeof(ResultForm), windowName) as ResultForm;

            if(f.Calulating) {
                f.Activate();
                MessageBox.Show(this, messageCalculationRunning);
                this.Activate();
            }
            else {
                f.SetExpression(e.Expression);
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
        /// Pøi uzavírání okna musíme uzavøít všechny pøidružené formuláøe
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            e.Cancel = !(this.CheckForChanges() && this.CloseChildWindow());

            if(e.Cancel)
                (this.MdiParent as MainForm).OpenedFileNames.Clear();
            else if(e.CloseReason == CloseReason.MdiFormClosing && this.fileName != null && this.fileName != string.Empty)
                (this.MdiParent as MainForm).OpenedFileNames.Add(this.fileName);
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
            // Hlavièka
            if(export.Binary) {
                export.B.Write((this.MdiParent as MainForm).RegistryEntryName);
                export.B.Write(3);
            }
            else {
                export.T.WriteLine((this.MdiParent as MainForm).RegistryEntryName);
                export.T.WriteLine(3);
            }

            IEParam param = new IEParam();

            param.Add(this.Location.X, "X");
            param.Add(this.Location.Y, "Y");
            param.Add(this.Size.Width, "Width");
            param.Add(this.Size.Height, "Height");

            param.Add(this.txtCommand.Rtf, "Text");
            param.Add(this.txtCommand.SelectionStart, "SelectionStart");
            param.Add(this.txtCommand.SelectionLength, "SelectionLength");
            param.Add(this.resultNumber, "ResultNumber");

            param.Add(this.context, "Context");

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            if(import.Binary) {
                import.VersionName = import.B.ReadString();
                import.VersionNumber = import.B.ReadInt32();
            }
            else {
                import.VersionName = import.T.ReadLine();
                import.VersionNumber = int.Parse(import.T.ReadLine());
            }

            if(import.VersionNumber < 3) {
                this.Location = new Point(import.B.ReadInt32(), import.B.ReadInt32());
                this.Size = new Size(import.B.ReadInt32(), import.B.ReadInt32());

                this.txtCommand.Rtf = import.B.ReadString();
                this.txtCommand.SelectionStart = import.B.ReadInt32();
                this.txtCommand.ScrollToCaret();

                this.context = new Context();
            }
            else {
                IEParam param = new IEParam(import);

                this.Location = new Point((int)param.Get(0), (int)param.Get(0));
                this.Size = new Size((int)param.Get(this.Size.Width), (int)param.Get(this.Size.Height));

                this.txtCommand.Rtf = (string)param.Get(this.txtCommand.Rtf);
                this.txtCommand.SelectionStart = (int)param.Get(0);
                this.txtCommand.SelectionLength = (int)param.Get(0);
                this.txtCommand.ScrollToCaret();

                this.resultNumber = (int)param.Get(0);

                this.context = (Context)param.Get();
            }

            this.InitializeEvents(this.context);
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
