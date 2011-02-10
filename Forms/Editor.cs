using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

using PavelStransky.Expression;
using PavelStransky.Core;
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

        private Thread exportThread;
        private Export export;

        private bool saving = false;
        private bool modifiedDuringSaving = false;
        private bool closeAfterSave = false;

        /// <summary>
        /// Kontext
        /// </summary>
        public Context Context { 
            get {
                this.context.Directory = WinMain.Directory;
                return this.context; 
            } 
        }

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
                if(value) {
                    if(this.saving)
                        this.modifiedDuringSaving = true;
                    else
                        this.lblSave.Visible = false;
                }

                if(this.modified != value) {
                    if(!this.saving && this.modifiedDuringSaving) {
                        this.modifiedDuringSaving = false;
                        if(!value)
                            value = true;
                    }

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
                this.fileName = value;
                this.SetCaption();
            }
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
        /// Inicializuje eventy pro context
        /// </summary>
        /// <param name="context">Kontext</param>
        private void InitializeEvents(Context context) {
            context.ContextEvent += new Context.ContextEventHandler(context_ContextEvent);
        }

        private delegate void ContextEventDelegate(ContextEventArgs e);

        /// <summary>
        /// Událost na kontextu
        /// </summary>
        void context_ContextEvent(object sender, ContextEventArgs e) {
            this.Invoke(new ContextEventDelegate(this.ContextEvent), e);
        }

        /// <summary>
        /// Událost na kontextu spouštíme ve vlastním vláknì
        /// </summary>
        private void ContextEvent(ContextEventArgs e) {
            switch(e.EventType) {
                case ContextEventType.Change: 
                    this.Modified = true; 
                    break;

                case ContextEventType.Save:
                    string fileName = e.GetParam() as string;

                    if(fileName != null && fileName != string.Empty)
                        this.Save(fileName);
                    else
                        this.Save();

                    break;

                case ContextEventType.SetContext:
                    this.context = e.GetParam() as Context;
                    break;

                case ContextEventType.NewContext:
                    this.InitializeEvents(e.GetParam() as Context);
                    break;

                case ContextEventType.GraphRequest:
                    TArray graphs = e.GetParam(0) as TArray;
                    string name = e.GetParam(1) as string;
                    int numColumns = (int)e.GetParam(2);

                    PointD position = (PointD)e.GetParam(3);
                    PointD size = (PointD)e.GetParam(4);

                    SingleGraphForm graphForm = this.NewParentForm(typeof(SingleGraphForm), name) as SingleGraphForm;
                    graphForm.SetGraph(graphs, numColumns);

                    graphForm.Location = new Point(graphForm.Location.X, graphForm.Location.Y);

                    if(position.X >= 0.0)
                        graphForm.Location = new Point((int)position.X, graphForm.Location.Y);

                    if(position.Y >= 0.0)
                        graphForm.Location = new Point(graphForm.Location.X, (int)position.Y);

                    if(size.X > 0.0) {
                        graphForm.ClientSize = new Size((int)size.X, graphForm.ClientSize.Height);
                        graphForm.RealWidth = (int)size.X;
                    }
                    else
                        graphForm.RealWidth = 0;

                    if(size.Y > 0.0) {
                        graphForm.ClientSize = new Size(graphForm.ClientSize.Width, (int)size.Y);
                        graphForm.RealHeight = (int)size.Y;
                    }
                    else
                        graphForm.RealHeight = 0;

                    graphForm.Show();
                    this.Activate();

                    break;

                case ContextEventType.Exit:
                    this.Close();
                    break;
            }
            
        }

        /// <summary>
        /// Nastaví eventy všech ResultForms (je nutné po otevøení ze souboru)
        /// </summary>
        public void SetResultFormsEvents() {
            Form[] forms = this.MdiParent.MdiChildren;
            for(int i = 0; i < forms.Length; i++) {
                ResultForm rf = forms[i] as ResultForm;

                if(rf != null && rf.ParentEditor == this) {
                    rf.CalcStarted += new EventHandler(this.Editor_CalcStarted);
                    rf.CalcFinished += new ResultForm.FinishedEventHandler(this.Editor_CalcFinished);
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

            if(type == typeof(SingleGraphForm)) {
                result = new SingleGraphForm();
                result.Location = new Point(margin, this.MdiParent.Height - result.Size.Height - 8 * margin);
            }
            else if(type == typeof(ResultForm)) {
                result = new ResultForm();
                result.Location = new Point(this.MdiParent.Width - result.Size.Width - 2 * margin, margin);
                (result as ResultForm).CalcStarted += new EventHandler(this.Editor_CalcStarted);
                (result as ResultForm).CalcFinished += new ResultForm.FinishedEventHandler(this.Editor_CalcFinished);
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
        private void Editor_CalcFinished(object sender, FinishedEventArgs e) {
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
                    if(resultForm != null && resultForm.Calculating) {
                        DialogResult result = MessageBox.Show(this,
                            string.Format(Messages.MClose, resultForm.Name),
                            Messages.MCloseCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

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
        /// <returns>-1, pokud rušíme akci, 0, pokud neukládáme, 1 pokud ukládáme</returns>
        private int CheckForChanges() {
            if(this.Modified) {
                DialogResult result = MessageBox.Show(this,
                    this.fileName == string.Empty ? Messages.MChanged : string.Format(Messages.MFileChanged, this.fileName),
                    Messages.MChangedCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if(result == DialogResult.Cancel)
                    return -1;
                else if(result == DialogResult.No)
                    return 0;
                else if(result == DialogResult.Yes) {
                    if(this.Save())
                        return 1;
                    else
                        return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Uloží pøíkazy; pokud zatím neznáme jméno souboru, ukáže dialog
        /// </summary>
        /// <returns></returns>
        public bool Save() {
            if(this.fileName == null || this.fileName == string.Empty) {
                this.saveFileDialog.InitialDirectory = WinMain.Directory;

                if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                    WinMain.SetDirectoryFromFile(saveFileDialog.FileName);
                    return this.Save(this.saveFileDialog.FileName);
                }
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
            this.saveFileDialog.InitialDirectory = WinMain.Directory;

            if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                WinMain.SetDirectoryFromFile(saveFileDialog.FileName);
                return this.Save(this.saveFileDialog.FileName);
            }
            else
                return false;
        }

        /// <summary>
        /// Uloží pøíkazy do souboru
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        /// <returns>False, pokud se uložení nezdaøilo</returns>
        public bool Save(string fileName) {
            // Upravení jména souboru
            if(!Path.IsPathRooted(fileName))
                fileName = Path.Combine(WinMain.Directory, fileName);

            if(!Path.HasExtension(fileName))
                fileName = string.Format("{0}.{1}", fileName, WinMain.FileExtGcm);

            this.FileName = fileName;

            this.saving = true;
            this.modifiedDuringSaving = false;

            this.exportThread = new Thread(this.ExportThreadStart);
            this.exportThread.IsBackground = true;
            this.exportThread.Start(fileName);

            return true;
        }

        private void ExportThreadStart(object o) {         
            string fileName = (string)o;
            string fileNameSave = fileName + ".sav";

            bool success = false;

            try {
                this.export = new Export(fileNameSave, IETypes.Compressed, versionNumber, WinMain.RegistryEntryName);
                this.export.ExportCommand += new Export.ExportEventHandler(export_ExportCommand);

                this.export.Write(this);

                Form parentForm = this.MdiParent;

                int num = 0;
                for(int i = 0; i < parentForm.MdiChildren.Length; i++) {
                    ChildForm childForm = parentForm.MdiChildren[i] as ChildForm;
                    if(childForm as IExportable != null && childForm.ParentEditor == this)
                        num++;
                }

                this.export.B.Write(num);
                for(int i = 0; i < parentForm.MdiChildren.Length; i++) {
                    ChildForm childForm = parentForm.MdiChildren[i] as ChildForm;
                    if(childForm as IExportable != null && childForm.ParentEditor == this)
                        this.export.Write(childForm);
                }

                this.export.Close();

                // Doèasný soubor, do kterého jsme ukládali, pøejmenujeme na orig. verzi
                if(File.Exists(fileName))
                    File.Delete(fileName);
                File.Move(fileNameSave, fileName);

                success = true;
            }
            catch(Exception e) {
                DetailException de = e as DetailException;

                export.Close();

                // Doèasný soubor, do kterého jsme ukládali, smažeme
                if(File.Exists(fileNameSave))
                    File.Delete(fileNameSave);

                MessageBox.Show(this, string.Format(Messages.EMSaveFailed, fileName, de != null ? de.GetText("\n\n") : e.Message),
                    Messages.EMSaveFailedCaption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                success = false;
            }

            this.OnFileSaved(new FileNameEventArgs(fileName, success));
            return;
        }

        private delegate void ExportCommandCallback(object sender, ExportEventArgs e);

        void export_ExportCommand(object sender, ExportEventArgs e) {
            if(this.lblSave.InvokeRequired) {
                ExportCommandCallback ec = new ExportCommandCallback(this.export_ExportCommand);
                this.Invoke(ec, new object[] { sender, e });
            }
            else {
                this.lblSave.Visible = true;
                this.lblSave.Text = e.TypeName;
            }
        }

        public event FileNameEventHandler FileSaved;

        /// <summary>
        /// Voláno pøi uložení souboru
        /// </summary>
        protected virtual void OnFileSaved(FileNameEventArgs e) {
            if(this.FileSaved != null)
                this.FileSaved(this, e);
            this.saving = false;

            if(e.Success)
                this.ClearModified();
        }

        private delegate void ClearModifiedCallback();
        private void ClearModified() {
            if(this.InvokeRequired) {
                ClearModifiedCallback e = new ClearModifiedCallback(this.ClearModified);
                this.Invoke(e);
            }
            else {
                if(this.closeAfterSave) {
                    if((this.MdiParent as MainForm).CloseAfterSave)
                        this.MdiParent.Close();
                    else
                        this.Close();
                }
                else
                    this.Modified = false;
            }
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

            if(f.Calculating) {
                f.Activate();
                MessageBox.Show(this, Messages.EMRun);
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

            if(this.saving) {
                e.Cancel = true;
                return;
            }

            int result = 0;
            if(!this.closeAfterSave) {
                this.closeAfterSave = false;
                (this.MdiParent as MainForm).CloseAfterSave = false;
                result = this.CheckForChanges();
            }
            if(result == -1)
                e.Cancel = true;
            else if(result == 0)
                e.Cancel = !this.CloseChildWindow();
            else if(result == 1) {
                e.Cancel = true;
                this.closeAfterSave = true;
            }

            if(e.Cancel)
                (this.MdiParent as MainForm).OpenedFileNames.Clear();
            else {
                this.context.Clear();
                if(e.CloseReason == CloseReason.MdiFormClosing && this.fileName != null && this.fileName != string.Empty)
                    (this.MdiParent as MainForm).OpenedFileNames.Add(this.fileName);
            }
        }

        /// <summary>
        /// Pøi zmìnì textu zmìníme atribut Modified
        /// </summary>
        private void txtCommand_TextChanged(object sender, System.EventArgs e) {
            this.Modified = txtCommand.Modified;
        }

        /// <summary>
        /// Pøi zmìnì zvýrazòování syntaxe
        /// </summary>
        private void chkHighlightSyntax_CheckedChanged(object sender, EventArgs e) {
            this.txtCommand.Highlighting = this.chkHighlightSyntax.Checked;
        }

        /// <summary>
        /// Tlaèítko spuštìní výpoètu
        /// </summary>
        private void btStart_Click(object sender, EventArgs e) {
            this.txtCommand.RunSelectedText();
            this.txtCommand.Focus();
        }

        #region Nápovìda pomocí ToolTipu
        // Objekt, ze kterého se bude zobrazovat toolTip
        private Highlight.HighlightItem toolTipItem;

        private void txtCommand_HighlightItemPointed(object sender, HighlightItemEventArgs e) {
            // Help tvoøíme vždy se zpoždìním podle èasovaèe tHelp
            // (aby pøi rychlých zmìnách nedocházelo ke zbyteèným výpoètùm)
            this.tHelp.Enabled = false;

            this.toolTipItem = e.HighlightItem;
            this.SetToolTipHelp(null);

            this.tHelp.Start();
        }

        /// <summary>
        /// Tick - nastavíme toolTipHelp
        /// </summary>
        private void tHelp_Tick(object sender, EventArgs e) {
            this.tHelp.Enabled = false;
            this.SetToolTipHelp(this.toolTipItem);
        }

        /// <summary>
        /// Nastaví nápovìdu do ToolTipu
        /// </summary>
        private void SetToolTipHelp(Highlight.HighlightItem item) {
            string help = null;

            if(item != null) {
                if(item.HighlightType == HighlightTypes.Function) {
                    help = Atom.GetHelp(item.Comment as string);
                }

                else if(item.HighlightType == HighlightTypes.Variable) {
                    if(this.context.Contains(item.Comment as string)) {
                        help = this.context[item.Comment as string].Item.GetType().Name;
                    }
                }
            }

            if(help != null) {
                // Maximálnì 20 øádek nebo 1000 znakù
                int i = -1;
                int n = 0;

                while((i = help.IndexOf('\n', i + 1)) >= 0)
                    if(++n > 20)
                        break;

                if(i > 0)
                    help = string.Format("{0}{1}...", help.Substring(0, i - 1), Environment.NewLine);

                if(help.Length > 1000)
                    help = string.Format("{0}...", help.Substring(0, 500));
            }

            this.toolTip.SetToolTip(this.txtCommand, help);
        }
        #endregion

        #region Implementace IExportable
        private delegate void ExportCallback(IEParam param);

        /// <summary>
        /// Uloží obsah kontextu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            if(this.InvokeRequired) {
                ExportCallback e = new ExportCallback(this.Export1);
                this.Invoke(e, new object[] { param });
            }
            else this.Export1(param);

            param.Add(this.context, "Context");

            if(this.InvokeRequired) {
                ExportCallback e = new ExportCallback(this.Export2);
                this.Invoke(e, new object[] { param });
            }
            else this.Export2(param);

            param.Export(export);
        }
        
        private void Export1(IEParam param) {
            param.Add(this.Location.X, "X");
            param.Add(this.Location.Y, "Y");
            param.Add(this.Size.Width, "Width");
            param.Add(this.Size.Height, "Height");

            param.Add(this.txtCommand.Rtf, "Text");
            param.Add(this.txtCommand.SelectionStart, "SelectionStart");
            param.Add(this.txtCommand.SelectionLength, "SelectionLength");
            param.Add(this.resultNumber, "ResultNumber");
        }

        private void Export2(IEParam param) {
            param.Add(this.WindowState.ToString(), "WindowState");
            param.Add(this.chkHighlightSyntax.Checked, "HighlightSyntax");
        }

        /// <summary>
        /// Naète obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Editor(Core.Import import)
            : this() {
            if(import.Binary) {
                if(import.VersionNumber <= 8) {
                    import.B.ReadString();
                    import.SetVersionNumber(import.B.ReadInt32());
                }
            }
            else {
                string[] line = import.T.ReadLine().Split('\t');
                import.SetVersionNumber(int.Parse(line[0]));
            }

            if(import.VersionNumber < 3) {
                this.Location = new Point(import.B.ReadInt32(), import.B.ReadInt32());
                this.Size = new Size(import.B.ReadInt32(), import.B.ReadInt32());

                this.txtCommand.Rtf = import.B.ReadString();
                this.txtCommand.SelectionStart = import.B.ReadInt32();
                this.txtCommand.ScrollToCaret();

                try {
                    this.context = new Context();
                }
                catch(Exception e) {
                    this.context = new Context();
                    DialogResult result = MessageBox.Show(this,
                        string.Format(Messages.EMContextOpen, e.Message),
                        Messages.MContextOpenCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else {
                IEParam param = new IEParam(import);

                this.Location = new Point((int)param.Get(0), (int)param.Get(0));
                this.Size = new Size((int)param.Get(this.Size.Width), (int)param.Get(this.Size.Height));

                this.txtCommand.Rtf = (string)param.Get(this.txtCommand.Rtf);
                this.txtCommand.SelectionStart = (int)param.Get(0);
                this.txtCommand.SelectionLength = (int)param.Get(0);
                this.txtCommand.ScrollToCaret();

                this.txtCommand.ForceHighlightSyntax();
                this.Modified = false;

                this.resultNumber = (int)param.Get(0);

                this.context = (Context)param.Get();

                this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), (string)param.Get(FormWindowState.Normal.ToString()), true);
                this.chkHighlightSyntax.Checked = (bool)param.Get(true);
            }

            this.InitializeEvents(this.context);
        }
        #endregion

        #region Drag and Drop obsluha
        private void txtCommand_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left && this.txtCommand.SelectionLength > 0 && e.Clicks == 1) {
                int selectionStart = this.txtCommand.SelectionStart;
                int selectionLength = this.txtCommand.SelectionLength;
                int firstShowedChar = this.txtCommand.GetCharIndexFromPosition(new Point(this.txtCommand.Margin.Left, this.txtCommand.Margin.Top));

                this.txtCommand.StopRedrawing();

                //invoke the drag and drop operation
                this.txtCommand.DoDragDrop(this.txtCommand.SelectedText, DragDropEffects.Copy);

                this.txtCommand.SelectionStart = firstShowedChar;
                this.txtCommand.ScrollToCaret();

                this.txtCommand.SelectionStart = selectionStart;
                this.txtCommand.SelectionLength = selectionLength;

                this.txtCommand.ResumeRedrawing();
                this.txtCommand.Invalidate();
            }
        }
        #endregion

        private const string defaultResultWindowName = "Result{0}";
        private const int versionNumber = 9;

        private const string defaultName = "Utitled";
        private const string titleFormatFile = "{1} {2}";
        private const string titleFormat = "{0} {2}";
        private const string asterisk = "*";

        private const string tmpFile = "tmp.tmp";

        private const int margin = 8;
    }
}
