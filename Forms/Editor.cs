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
        private static int resultNumber = 0;

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
                this.Directory = this.DirectoryFromFile(value);
                this.fileName = value;
                this.SetCaption();
            }
        }

        /// <summary>
        /// Nastav� / vyzvedne aktu�ln� adres��
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
            this.context = new Context();
            this.InitializeEvents();
            this.SetCaption();
        }

        /// <summary>
        /// Ud�lost z kontextu - ��dost o uzav�en� okna
        /// </summary>
        private void context_ExitRequest(object sender, EventArgs e) {
            this.Close();
        }

        #region Obsluha grafu
        private delegate void GraphRequestDelegate(GraphRequestEventArgs e);

        /// <summary>
        /// Ud�lost z kontextu - ��dost o vytvo�en� grafu
        /// </summary>
        private void context_GraphRequest(object sender, GraphRequestEventArgs e) {
            // Spust�me ve vlastn�m threadu
            this.Invoke(new GraphRequestDelegate(this.CreateGraph), new object[] { e });
        }

        /// <summary>
        /// Vytvo�en� nov�ho formul��e mus�me spustit ve vlastn�m threadu
        /// </summary>
        private void CreateGraph(GraphRequestEventArgs e) {
            GraphForm graphForm = this.NewParentForm(typeof(GraphForm), e.Name) as GraphForm;
            graphForm.SetGraph(e.Graphs, e.NumColumns);

            graphForm.Show();
            this.Activate();
        }

        #endregion

        /// <summary>
        /// Vytvo�� nov� pod��zen� formul��
        /// </summary>
        /// <param name="type">Typ formul��e</param>
        /// <param name="name">N�zev okna</param>
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
                if(this.saveFileDialog.ShowDialog() == DialogResult.OK)
                    return this.Save(this.saveFileDialog.FileName);
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

            if(this.saveFileDialog.ShowDialog() == DialogResult.OK)
                return this.Save(this.saveFileDialog.FileName);
            else
                return false;
        }

        /// <summary>
        /// Ulo�� p��kazy do souboru
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        /// <returns>False, pokud se ulo�en� nezda�ilo</returns>
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

            return true;
        }
        #endregion

        /// <summary>
        /// P�i ��dosti o spu�t�n� p��kazu
        /// </summary>
        private void txtCommand_ExecuteCommand(object sender, PavelStransky.Forms.ExecuteCommandEventArgs e) {
            // Chceme nov� okno
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
        /// P�i zm�n� textu zm�n�me atribut Modified
        /// </summary>
        private void txtCommand_TextChanged(object sender, System.EventArgs e) {
            this.Modified = txtCommand.Modified;
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

        /// <summary>
        /// Podle n�zvu souboru ur�� adres��
        /// </summary>
        /// <param name="fileName">N�zev souboru s cestou</param>
        private string DirectoryFromFile(string fileName) {
            FileInfo f = new FileInfo(fileName);
            return f.DirectoryName;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah kontextu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            // Mus�me ukl�dat bin�rn�
            if(!export.Binary)
                throw new Exception("");

            // Bin�rn�
            BinaryWriter b = export.B;
            b.Write((this.MdiParent as MainForm).RegistryEntryName);
            b.Write(1);                 // Rezervovan�

            b.Write(this.Location.X);
            b.Write(this.Location.Y);
            b.Write(this.Size.Width);
            b.Write(this.Size.Height);

            b.Write(this.txtCommand.Rtf);
            b.Write(this.txtCommand.SelectionStart);

            export.Write(this.context);
        }

        /// <summary>
        /// Na�te obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            // Mus�me ��st bin�rn�
            if(!import.Binary)
                throw new Exception("");

            // Bin�rn�
            BinaryReader b = import.B;
            import.VersionName = b.ReadString();                 // Ozna�en� verze (zat�m nekontrolujeme)
            import.VersionNumber = b.ReadInt32();                // ��slo verze

            this.Location = new Point(b.ReadInt32(), b.ReadInt32());
            this.Size = new Size(b.ReadInt32(), b.ReadInt32());

            this.txtCommand.Rtf = b.ReadString();
            this.txtCommand.SelectionStart = b.ReadInt32();

            if(import.VersionNumber >= 1)
                this.context = import.Read() as Context;

            this.InitializeEvents();
        }
        #endregion

        private const string directoryVariable = "_dir";

        private const string messageOpen = "Chcete, aby byly v�echny p��kazy historie po otev�en� automaticky spu�t�ny?";
        private const string captionOpen = "Otev�en� historie";

        private const string messageFileChanged = "Soubor '{0}' byl zm�n�n. Chcete zm�ny ulo�it?";
        private const string messageChanged = "Data nejsou ulo�ena. Chcete je ulo�it?";
        private const string captionFileChanged = "Ulo�en� souboru";

        private const string messageFailedSave = "Ulo�en� do souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}";
        private const string messageFailedSaveDetail = "Ulo�en� do souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedSave = "Chyba!";

        private const string messageClose = "V okn� {0} prob�h� v�po�et. Opravdu chcete okno uzav��t a v�po�et ukon�it?";
        private const string captionClose = "Varov�n�";

        private const string messageCalculationRunning = "V aktu�ln�m okn� prob�h� v�po�et, nelze spustit nov� v�po�et!";
        private const string defaultResultWindowName = "Result{0}";

        private const string defaultName = "Utitled";
        private const string titleFormatFile = "{1} {2}";
        private const string titleFormat = "{0} {2}";
        private const string asterisk = "*";

        private const int margin = 8;
    }
}
