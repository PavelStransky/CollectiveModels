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
    /// Formul�� pro editaci
    /// </summary>
    public partial class Editor : Form, IExportable {
        //Kontext s v�razy
        private Context context;

        // Jm�no souboru
        private string fileName = string.Empty;

        // Do�lo k modifikaci?
        private bool modified = false;

        // ��slo v�sledkov�ho formul��e
        private int resultNumber = 0;

        /// <summary>
        /// Kontext
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Dialog Ulo�it
        /// </summary>
        public SaveFileDialog SaveFileDialog { get { return this.saveFileDialog; } }

        /// <summary>
        /// Nefunguje dob�e ud�lost txtCommand_OnModifiedChanged, proto mus�me vy�e�it po sv�m
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
        /// P�e�te / nastav� jm�no souboru
        /// </summary>
        public string FileName {
            get {
                return this.fileName;
            }
            set {
                if(Path.IsPathRooted(fileName))
                    this.Directory = Path.GetDirectoryName(fileName);

                this.fileName = value;
                this.SetCaption();
            }
        }

        /// <summary>
        /// Nastav� / vyzvedne aktu�ln� adres��
        /// </summary>
        public string Directory {
            get {
                if(this.context.Directory != string.Empty)
                    return this.context.Directory;
                else {
                    FileInfo f = new FileInfo(tmpFile);
                    return f.DirectoryName;
                }
            }

            set {
                this.Context.Directory = value;
                this.saveFileDialog.InitialDirectory = value;
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
        /// Ud�lost na kontextu
        /// </summary>
        void context_ContextEvent(object sender, ContextEventArgs e) {
            this.Invoke(new ContextEventDelegate(this.ContextEvent), e);
        }

        /// <summary>
        /// Ud�lost na kontextu spou�t�me ve vlastn�m vl�kn�
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

                    GraphForm graphForm = this.NewParentForm(typeof(GraphForm), name) as GraphForm;
                    graphForm.SetGraph(graphs, numColumns);

                    graphForm.Location = new Point(graphForm.Location.X, graphForm.Location.Y);

                    if(position.X >= 0.0)
                        graphForm.Location = new Point((int)position.X, graphForm.Location.Y);
                    if(position.Y >= 0.0)
                        graphForm.Location = new Point(graphForm.Location.X, (int)position.Y);
                    if(size.X > 0.0)
                        graphForm.Size = new Size((int)size.X, graphForm.Size.Height);
                    if(size.Y > 0.0)
                        graphForm.Size = new Size(graphForm.Size.Width, (int)size.Y);


                    graphForm.Show();
                    this.Activate();

                    break;

                case ContextEventType.Exit:
                    this.Close();
                    break;
            }
            
        }

        /// <summary>
        /// Nastav� eventy v�ech ResultForms (je nutn� po otev�en� ze souboru)
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
        /// Pokus� se nal�zt formul�� s dan�m n�zvem a typem
        /// </summary>
        /// <param name="type">Typ formul��e</param>
        /// <param name="name">N�zev okna</param>
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
        /// Vytvo�� nov� pod��zen� formul��
        /// </summary>
        /// <param name="type">Typ formul��e</param>
        /// <param name="name">N�zev okna</param>
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
        /// Zav�en� formul��e s v�sledky
        /// </summary>
        private void result_FormClosed(object sender, FormClosedEventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null) 
                this.mrbResult.Remove(r.Name);
        }

        /// <summary>
        /// Za��tek v�po�tu
        /// </summary>
        private void Editor_CalcStarted(object sender, EventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null) 
                this.mrbResult.SetBackColor(r.Name, Color.Red);
        }

        /// <summary>
        /// Konec v�po�tu
        /// </summary>
        private void Editor_CalcFinished(object sender, FinishedEventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null)
                this.mrbResult.SetDefaultBackColor(r.Name);
        }

        /// <summary>
        /// P�eru�en� v�po�et
        /// </summary>
        private void Editor_CalcPaused(object sender, EventArgs e) {
            ResultForm r = sender as ResultForm;

            if(r != null)
                this.mrbResult.SetBackColor(r.Name, Color.Blue);
        }

        /// <summary>
        /// Kliknut� na RadioButton
        /// </summary>
        private void mrbResult_RBClick(object sender, MultipleRadioButtonEventArgs e) {
            ResultForm result = this.GetChildFormFromName(typeof(ResultForm), e.Name) as ResultForm;

            if(result != null) {
                result.Activate();
                this.Activate();
            }
        }

        #region Otev�r�n� a ukl�d�n� p��kaz�
        /// <summary>
        /// Pokus� se zav��t v�echna pod��zen� okna
        /// </summary>
        /// <returns>True, pokud je v�e OK a uzav�en� se poda�ilo</returns>
        private bool CloseChildWindow() {
            MainForm form = this.MdiParent as MainForm;

            for(int i = 0; i < form.MdiChildren.Length; i++) {
                // V�echny pod��zen� formul��e
                ChildForm childForm = form.MdiChildren[i] as ChildForm;

                if(childForm != null && childForm.ParentEditor == this) {
                    ResultForm resultForm = childForm as ResultForm;
                    if(resultForm != null && resultForm.Calculating) {
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

                    // Vymazali jsme formul��, proto mus�me vr�tit index zp�t
                    i--;
                }
            }

            return true;
        }

        /// <summary>
        /// Kontroluje, zda do�lo ke zm�n�m. Pokud ano, vy�le dotaz na ulo�en�
        /// </summary>
        /// <returns>False, pokud je ��dost o zru�en� akce</returns>
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
        /// Ulo�� p��kazy; pokud zat�m nezn�me jm�no souboru, uk�e dialog
        /// </summary>
        /// <returns></returns>
        public bool Save() {
            if(this.fileName == null || this.fileName == string.Empty) {
                if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                    this.Directory = Path.GetDirectoryName(saveFileDialog.FileName);
                    return this.Save(this.saveFileDialog.FileName);
                }
                else
                    return false;
            }
            else
                return this.Save(this.fileName);
        }

        /// <summary>
        /// Ulo�� p��kazy a v�dy zobraz� dialog
        /// </summary>
        public bool SaveAs() {
            this.saveFileDialog.FileName = this.fileName;

            if(this.saveFileDialog.ShowDialog() == DialogResult.OK) {
                this.Directory = Path.GetDirectoryName(saveFileDialog.FileName);
                return this.Save(this.saveFileDialog.FileName);
            }
            else
                return false;
        }

        /// <summary>
        /// Ulo�� p��kazy do souboru
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        /// <returns>False, pokud se ulo�en� nezda�ilo</returns>
        public bool Save(string fileName) {
            // Upraven� jm�na souboru
            if(!Path.IsPathRooted(fileName))
                fileName = Path.Combine(this.Directory, fileName);

            if(!Path.HasExtension(fileName))
                fileName = string.Format("{0}.{1}", fileName, WinMain.FileExtGcm);

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

                // Do�asn� soubor, do kter�ho jsme ukl�dali, p�ejmenujeme na orig. verzi
                if(File.Exists(fileName))
                    File.Delete(fileName);
                File.Move(fileNameSave, fileName);

                this.Modified = false;            
                this.SetCaption();
            }
            catch(DetailException e) {
                export.Close();
                
                // Do�asn� soubor, do kter�ho jsme ukl�dali, sma�eme
                if(File.Exists(fileNameSave))
                    File.Delete(fileNameSave);

                MessageBox.Show(this, string.Format(messageFailedSaveDetail, fileName, e.Message, e.DetailMessage),
                    captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch(Exception e) {
                export.Close();

                // Do�asn� soubor, do kter�ho jsme ukl�dali, sma�eme
                if(File.Exists(fileNameSave))
                    File.Delete(fileNameSave);

                MessageBox.Show(this, string.Format(messageFailedSave, fileName, e.Message),
                    captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            this.OnFileSaved(new FileNameEventArgs(fileName));
            return true;
        }

        public event FileNameEventHandler FileSaved;

        /// <summary>
        /// Vol�no p�i ulo�en� souboru
        /// </summary>
        protected virtual void OnFileSaved(FileNameEventArgs e) {
            if(this.FileSaved != null)
                this.FileSaved(this, e);
        }
        #endregion

        /// <summary>
        /// P�i ��dosti o spu�t�n� p��kazu
        /// </summary>
        private void txtCommand_ExecuteCommand(object sender, PavelStransky.Forms.ExecuteCommandEventArgs e) {
            string windowName = null;
            if(!e.NewWindow && !this.mrbResult.IsNewChecked)
                windowName = this.mrbResult.GetCheckedName();

            // Chceme nov� okno
            if(windowName == null)
                windowName = string.Format(defaultResultWindowName, ++this.resultNumber);

            ResultForm f = this.NewParentForm(typeof(ResultForm), windowName) as ResultForm;

            if(f.Calculating) {
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
        /// nastav� nadpis okna
        /// </summary>
        private void SetCaption() {
            this.Text = string.Format(this.fileName == string.Empty ? titleFormat : titleFormatFile,
                defaultName, this.fileName, this.Modified ? asterisk : string.Empty);
        }

        /// <summary>
        /// P�i aktivaci okna nastav�me focus
        /// </summary>
        private void Editor_Activated(object sender, System.EventArgs e) {
            this.txtCommand.Focus();
        }

        /// <summary>
        /// P�i uzav�r�n� okna mus�me uzav��t v�echny p�idru�en� formul��e
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            e.Cancel = !(this.CheckForChanges() && this.CloseChildWindow());

            if(e.Cancel)
                (this.MdiParent as MainForm).OpenedFileNames.Clear();
            else if(e.CloseReason == CloseReason.MdiFormClosing && this.fileName != null && this.fileName != string.Empty)
                (this.MdiParent as MainForm).OpenedFileNames.Add(this.fileName);
        }

        #region N�pov�da pomoc� ToolTipu
        private string oldFnName;
        private int position;

        /// <summary>
        /// P�i zm�n� textu zm�n�me atribut Modified
        /// </summary>
        private void txtCommand_TextChanged(object sender, System.EventArgs e) {
            this.Modified = txtCommand.Modified;
            this.SetToolTipHelp();
        }

        /// <summary>
        /// Ud�lost vyvolan� p�i p�esunu my�i
        /// </summary>
        private void txtCommand_MouseMove(object sender, MouseEventArgs e) {
            this.position = this.txtCommand.GetCharIndexFromPosition(e.Location);
            this.SetToolTipHelp();
        }

        /// <summary>
        /// Nastav� n�pov�du do ToolTipu
        /// </summary>
        private void SetToolTipHelp() {
            string s = Atom.GetFnName(this.txtCommand.Text, this.position);

            if(this.oldFnName != s) {
                string help = Atom.GetHelp(s, 20);
                this.toolTip.SetToolTip(this.txtCommand as Control, help);
                this.oldFnName = s;
            }
        }
        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah kontextu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            // Hlavi�ka
            if(export.Binary) {
                export.B.Write(WinMain.RegistryEntryName);
                export.B.Write(5);
            }
            else {
                export.T.WriteLine(WinMain.RegistryEntryName);
                export.T.WriteLine(5);
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

            param.Add(this.WindowState.ToString(), "WindowState");

            param.Export(export);
        }

        /// <summary>
        /// Na�te obsah kontextu ze souboru
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

                try {
                    this.context = new Context();
                }
                catch(Exception e) {
                    this.context = new Context();
                    DialogResult result = MessageBox.Show(this,
                        string.Format(messageContextError, e.Message),
                        captionContextError, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                this.txtCommand.HighlightSyntax();

                this.resultNumber = (int)param.Get(0);

                this.context = (Context)param.Get();

                this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), (string)param.Get(FormWindowState.Normal.ToString()), true);
            }

            this.InitializeEvents(this.context);
        }
        #endregion

        #region Drag and Drop obsluha
        private void txtCommand_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left && this.txtCommand.SelectionLength > 0) {
                //invoke the drag and drop operation
                this.txtCommand.DoDragDrop(this.txtCommand.SelectedText, DragDropEffects.Copy);
            }
        }
        #endregion

        private const string messageFileChanged = "Soubor '{0}' byl zm�n�n. Chcete zm�ny ulo�it?";
        private const string messageChanged = "Data nejsou ulo�ena. Chcete je ulo�it?";
        private const string captionFileChanged = "Ulo�en� souboru";

        private const string messageFailedSave = "Ulo�en� do souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}";
        private const string messageFailedSaveDetail = "Ulo�en� do souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedSave = "Chyba!";

        private const string messageClose = "V okn� {0} prob�h� v�po�et. Opravdu chcete okno uzav��t a v�po�et ukon�it?";
        private const string captionClose = "Varov�n�";

        private const string messageContextError = "Nepoda�ilo se otev��t kontext.\n\nPodrobnosti: {0}";
        private const string captionContextError = "Chyba!";

        private const string messageCalculationRunning = "V aktu�ln�m okn� prob�h� v�po�et, nelze spustit nov� v�po�et!";
        private const string defaultResultWindowName = "Result{0}";

        private const string defaultName = "Utitled";
        private const string titleFormatFile = "{1} {2}";
        private const string titleFormat = "{0} {2}";
        private const string asterisk = "*";

        private const string tmpFile = "tmp.tmp";

        private const int margin = 8;
    }
}
