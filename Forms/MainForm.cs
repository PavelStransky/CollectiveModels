using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Forms {
    public partial class MainForm : Form {
        private ArrayList openedFileNames = new ArrayList();
        private LastOpenedFiles lastOpenedFiles;

        /// <summary>
        /// Otev�en� soubory p�i ukon�ov�n� aplikace
        /// </summary>
        public ArrayList OpenedFileNames { get { return this.openedFileNames; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainForm() {
            this.Initialize();
            this.Show();

            foreach(string fileName in this.openedFileNames)
                if(fileName != null && fileName != string.Empty)
                    this.Open(fileName);

            this.SetMenu();
            this.openedFileNames.Clear();
        }

        /// <summary>
        /// Konstruktor s otev�en�m souboru
        /// </summary>
        /// <param name="fileName">N�zev souboru</param>
        public MainForm(string fileName) {
            this.Initialize();
            this.Show();

            this.Open(fileName);

            this.SetMenu();
            this.openedFileNames.Clear();
        }

        /// <summary>
        /// Inicializace instance nov� GCM
        /// </summary>
        private void Initialize() {
            this.InitializeComponent();

            this.Menu = this.mnMenu;
            this.SetDialogProperties(this.openFileDialog);

            this.mnSetttingsRegistry.Checked = WinMain.IsRegistered;
            this.mnSettingsPlaySounds.Checked = WinMain.PlaySounds;

            this.Text = Application.ProductName;

            // Na�ten� z�znam� z registr�
            object x = WinMain.GetRegistryValue(registryKeyPositionX);
            object y = WinMain.GetRegistryValue(registryKeyPositionY);
            if(x is int && y is int && (int)x > 0 && (int)y > 0) {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point((int)x, (int)y);
            }
            else
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;

            object width = WinMain.GetRegistryValue(registryKeyWidth);
            object height = WinMain.GetRegistryValue(registryKeyHeight);

            if(width is int && height is int && (int)width > 0 && (int)height > 0)
                this.Size = new Size((int)width, (int)height);

            int i = 0;
            object openedFile;
            while((openedFile = WinMain.GetRegistryValue(string.Format(registryKeyOpenedFile, i++), true)) != null) 
                this.openedFileNames.Add(openedFile);

            object clearWriterInformation = WinMain.GetRegistryValue(registryKeyClearWriterInformation);
            if(clearWriterInformation is string)
                this.cmTrayClearWriter.Checked = bool.Parse(clearWriterInformation as string);

            object actualInformation = WinMain.GetRegistryValue(registryKeyActualInformation);
            if(actualInformation is string)
                this.cmTrayActualInformation.Checked = bool.Parse(actualInformation as string);

            object finishedInformation = WinMain.GetRegistryValue(registryKeyFinishedInformation);
            if(finishedInformation is string)
                this.cmTrayFinishInformation.Checked = bool.Parse(finishedInformation as string);

            this.lastOpenedFiles = new LastOpenedFiles(-1, this.mnFile);
            this.lastOpenedFiles.Click += new FileNameEventHandler(lastOpenedFiles_Click);
            this.FileOpened += new FileNameEventHandler(this.lastOpenedFiles.AddFile);
        }

        /// <summary>
        /// Nastav� vlastnosti dialogu
        /// </summary>
        /// <param name="dialog">Dialog</param>
        private void SetDialogProperties(FileDialog dialog) {
            dialog.Reset();
            dialog.Filter = WinMain.FileFilterGcm;
            dialog.DefaultExt = WinMain.FileExtGcm;
            dialog.InitialDirectory = WinMain.Directory;
            dialog.RestoreDirectory = false;
        }

        #region Menu Okno
        /// <summary>
        /// Se�adit okna
        /// </summary>
        private void MnCascade_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        /// <summary>
        /// Dla�dice horizont�ln�
        /// </summary>
        private void mnTileHorizontal_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        /// <summary>
        /// Dla�dice vertik�ln�
        /// </summary>
        private void mnTileVertical_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        /// <summary>
        /// Se�adit ikony
        /// </summary>
        private void mnArrangeIcons_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }
        #endregion

        #region Menu Soubor
        /// <summary>
        /// Nov� okno
        /// </summary>
        private void mnFileNew_Click(object sender, EventArgs e) {
            this.New();
        }

        /// <summary>
        /// Vytvo�� nev� okno s editorem
        /// </summary>
        private void New() {
            Editor editor = new Editor();
            this.SetDialogProperties(editor.SaveFileDialog);
            editor.Directory = WinMain.Directory;
            editor.MdiParent = this;
            editor.Show();

            editor.FormClosed += new FormClosedEventHandler(this.editor_FormClosed);
            editor.FileSaved += new FileNameEventHandler(this.lastOpenedFiles.AddFile);

            this.SetMenu();
        }

        /// <summary>
        /// Otev��t
        /// </summary>
        private void mnFileOpen_Click(object sender, EventArgs e) {
            this.openFileDialog.ShowDialog();
        }

        /// <summary>
        /// Otev�en� souboru - vol�no z dialogu FileOpen
        /// </summary>
        private void openFileDialog_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetDirectoryFromFile(this.openFileDialog.FileName);
            this.Open(this.openFileDialog.FileName);
            this.SetMenu();
        }

        /// <summary>
        /// P�i vybr�n� polo�ky ze seznamu naposledy otev�en�ch soubor�
        /// </summary>
        void lastOpenedFiles_Click(object sender, FileNameEventArgs e) {
            WinMain.SetDirectoryFromFile(e.FileName);
            this.Open(e.FileName);
            this.SetMenu();
        }

        /// <summary>
        /// Otev�e soubor
        /// </summary>
        /// <param name="fileName">N�zev souboru</param>
        private void Open(string fileName) {
            Import import = null;
            Editor editor = null;

            // P��d�n� p��pony
            if(!Path.HasExtension(fileName))
                fileName = string.Format("{0}.{1}", fileName, WinMain.FileExtGcm);

            try {
                import = new Import(fileName, true);
                editor = import.Read() as Editor;

                this.SetDialogProperties(editor.SaveFileDialog);
                editor.FormClosed += new FormClosedEventHandler(this.editor_FormClosed);
                editor.FileSaved += new FileNameEventHandler(this.lastOpenedFiles.AddFile);
                editor.MdiParent = this;
                editor.Show();

                // Pokud m�me spr�vnou verzi, na�teme ostatn� okna
                if(import.VersionNumber >= 3) {
                    int num = import.B.ReadInt32();
                    for(int i = 0; i < num; i++) {
                        object o = import.Read();
                        if(o is ChildForm) {
                            (o as ChildForm).ParentEditor = editor;
                            (o as ChildForm).MdiParent = this;
                            (o as ChildForm).Show();
                        }
                    }
                }

                editor.SetResultFormsEvents();
                editor.FileName = fileName;
                editor.Modified = false;
                editor.Activate();

                this.OnFileOpened(new FileNameEventArgs(fileName));
            }
            catch(DetailException e) {
                MessageBox.Show(this, string.Format(messageFailedOpenDetail, fileName, e.Message, e.DetailMessage),
                    captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                if(editor != null)
                    editor.Close();
            }
            catch(Exception e) {
                MessageBox.Show(this, string.Format(messageFailedOpen, fileName, e.Message),
                    captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                if(editor != null)
                    editor.Close();
            }

            try {
                import.Close();
            }
            catch { }
        }

        public event FileNameEventHandler FileOpened;

        /// <summary>
        /// Vol�no po �sp�n�m otev�en� souboru
        /// </summary>
        protected virtual void OnFileOpened(FileNameEventArgs e) {
            if(this.FileOpened != null)
                this.FileOpened(this, e);
        }

        /// <summary>
        /// Najde editor p��slu�ej�c� k aktivn�mu oknu
        /// </summary>
        /// <param name="form">Aktivn� okno</param>
        public Editor FindActiveMdiEditor(Form form) {
            if(form is Editor)
                return form as Editor;

            if(form is ChildForm) {
                for(int i = 0; i < this.MdiChildren.Length; i++) {
                    Editor editor = this.MdiChildren[i] as Editor;
                    if(editor != null && editor == (form as ChildForm).ParentEditor)
                        return editor;
                }
            }

            throw new FormsException(string.Format(errorMessageEditorNotFound, form.Name));
        }

        /// <summary>
        /// Nastav� menu podle stavu otev�en�ch oken
        /// </summary>
        public void SetMenu() {
            this.SetMenu(null);
        }

        /// <summary>
        /// Nastav� menu podle stavu otev�en�ch oken
        /// </summary>
        /// <param name="closed">Pr�v� uzav�ran� okno (ji� uva�ujeme, �e je uzav�en�)</param>
        private void SetMenu(Editor closed) {
            bool isEditor = false;

            // Hled�me otev�en� editor
            for(int i = 0; i < this.MdiChildren.Length; i++) {
                Editor editor = this.MdiChildren[i] as Editor;
                if(editor != null && editor != closed) {
                    isEditor = true;
                    break;
                }
            }

            if(isEditor) {
                this.mnWindow.Visible = true;
                this.mnFileClose.Visible = true;
                this.mnFileSave.Visible = true;
                this.mnFileSaveAs.Visible = true;
                this.mnFileSeparator2.Visible = true;
            }
            else {
                this.mnWindow.Visible = false;
                this.mnFileClose.Visible = false;
                this.mnFileSave.Visible = false;
                this.mnFileSaveAs.Visible = false;
                this.mnFileSeparator2.Visible = false;
            }
        }

        /// <summary>
        /// Ulo�it
        /// </summary>
        private void mnFileSave_Click(object sender, EventArgs e) {
            if(!this.mnFileSave.Visible)
                return;
            Editor editor = this.FindActiveMdiEditor(this.ActiveMdiChild);
            editor.Save();
        }

        /// <summary>
        /// Ulo�it jako
        /// </summary>
        private void mnFileSaveAs_Click(object sender, EventArgs e) {
            Editor editor = this.FindActiveMdiEditor(this.ActiveMdiChild);
            editor.SaveAs();
        }

        /// <summary>
        /// Zav��t
        /// </summary>
        private void mnFileClose_Click(object sender, EventArgs e) {
            Editor editor = this.FindActiveMdiEditor(this.ActiveMdiChild);
            editor.Close();
            this.SetMenu();
        }

        /// <summary>
        /// Ukon�� aplikaci
        /// </summary>
        private void mnExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// P�i ukon�en� ukl�d�me informace o aktu�ln�m okn�
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            WinMain.SetRegistryValue(registryKeyPositionX, this.Location.X);
            WinMain.SetRegistryValue(registryKeyPositionY, this.Location.Y);
            WinMain.SetRegistryValue(registryKeyWidth, this.Width);
            WinMain.SetRegistryValue(registryKeyHeight, this.Height);

            WinMain.SetRegistryValue(registryKeyActualInformation, this.cmTrayActualInformation.Checked);
            WinMain.SetRegistryValue(registryKeyClearWriterInformation, this.cmTrayClearWriter.Checked);
            WinMain.SetRegistryValue(registryKeyFinishedInformation, this.cmTrayFinishInformation.Checked);

            int i = 0;
            foreach(string fileName in this.openedFileNames)
                WinMain.SetRegistryValue(string.Format(registryKeyOpenedFile, i++), fileName);

            this.lastOpenedFiles.Save();

            WinMain.SaveSettings();
        }
        #endregion

        /// <summary>
        /// Uzav�en� editoru - p��padn� skryt� menu
        /// </summary>
        void editor_FormClosed(object sender, FormClosedEventArgs e) {
            this.SetMenu(sender as Editor);
        }

        #region Menu Nastaven�
        /// <summary>
        /// Registrace p��pony
        /// </summary>
        private void mnSetttingsRegistry_Click(object sender, EventArgs e) {
            this.mnSetttingsRegistry.Checked = !this.mnSetttingsRegistry.Checked;
            WinMain.Register(this.mnSetttingsRegistry.Checked);
        }

        /// <summary>
        /// P�ehr�vat zvuky
        /// </summary>
        private void mnSettingsPlaySounds_Click(object sender, EventArgs e) {
            this.mnSettingsPlaySounds.Checked = !this.mnSettingsPlaySounds.Checked;
            WinMain.PlaySounds = this.mnSettingsPlaySounds.Checked;
        }
        #endregion

        #region Menu Skr�t
        // Priorita p�ed minimalizov�n�m
        private ProcessPriority priority = ProcessPriority.Normal;
        private string balloonText = string.Empty;

        /// <summary>
        /// Minimalizace do System Tray
        /// </summary>
        private void mnHideToTray_Click(object sender, EventArgs e) {
            this.Hide();
            this.trayIcon.Visible = true;

            this.balloonText = string.Empty;

            string text = this.trayIcon.Text;
            int p = text.IndexOf(Environment.NewLine);
            if(p >= 0)
                text = text.Substring(0, p);

            foreach(Form form in this.MdiChildren) 
                if(form as Editor != null)
                    text += Environment.NewLine + Path.GetFileName((form as Editor).FileName);

            // Text m��e m�t maxim�ln� 64 znak�
            this.trayIcon.Text = text.Substring(0, System.Math.Min(text.Length, 64));

            foreach(Form rf in this.MdiChildren) 
                if(rf as ResultForm != null) {
                    if((rf as ResultForm).Calculating) {
                        (rf as ResultForm).CalcFinished += this.rf_CalcFinished;
                        (rf as ResultForm).TxtResult.TextChanged += new EventHandler(TxtResult_TextChanged);
                    }
                }

            // Nastaven� priority
            Process process = new Process();
            this.priority = process.GetPriority();
            process.SetPriority(ProcessPriority.Idle);
        }

        void TxtResult_TextChanged(object sender, EventArgs e) {
            TextBox textBox = sender as TextBox;

            if(textBox == null || !this.trayIcon.Visible)
                return;

            if(!this.cmTrayActualInformation.Checked && !this.cmTrayClearWriter.Checked)
                return;

            string text = textBox.Text;
            if(this.cmTrayClearWriter.Checked) {
                if(text.Length >= this.balloonText.Length) {
                    this.balloonText = text;
                    return;
                }
                else {
                    text = this.balloonText;
                    this.balloonText = string.Empty;

                    while(text.Substring(2) == Environment.NewLine)
                        text = text.Substring(text.Length - 2);
                    while(text.Substring(text.Length - 2) == Environment.NewLine)
                        text = text.Substring(0, text.Length - 2);

                    int p1 = text.IndexOf(Environment.NewLine);
                    int p2 = text.LastIndexOf(Environment.NewLine);

                    if(p1 > 0 && p1 != p2)
                        text = string.Format("{0}...{1}", text.Substring(0, p1 + Environment.NewLine.Length), text.Substring(p2));
                }
            }

            if(this.cmTrayActualInformation.Checked) {
                int p = text.Length;
                for(int i = 0; i < numBalloonLines && p > 0; i++)
                    p = text.LastIndexOf(Environment.NewLine, p - 1);

                if(p > 0)
                    text = string.Format("{0}...{1}", text.Substring(0, text.IndexOf(Environment.NewLine) + Environment.NewLine.Length), text.Substring(p));

                this.balloonText = text;
            }
            
            if(text.Length > 0)
                this.trayIcon.ShowBalloonTip(5000, "Prob�h� v�po�et...", text, ToolTipIcon.Info);
        }

        // Ukon�en� v�po�tu
        void rf_CalcFinished(object sender, FinishedEventArgs e) {
            this.balloonText = string.Empty;
            ResultForm rf = sender as ResultForm;

            if(rf == null || !this.trayIcon.Visible)
                return;

            if(this.cmTrayFinishInformation.Checked)
                this.trayIcon.ShowBalloonTip(10000, "Dokon�en v�po�et", rf.TxtCommand.Text, ToolTipIcon.Info);
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e) {
            // Zachyt�v�me pouze stisk lev�ho tla��tka my�i
            if(e.Button != MouseButtons.Left)
                return;

            this.trayIcon.Visible = false;
            this.Show();

            // Nastaven� priority
            Process process = new Process();
            if(process.GetPriority() == ProcessPriority.Idle)
                process.SetPriority(this.priority);

            foreach(Form rf in this.MdiChildren)
                if(rf as ResultForm != null) 
                    (rf as ResultForm).CalcFinished -= this.rf_CalcFinished;                
        }

        /// <summary>
        /// Aktu�ln� informace
        /// </summary>
        private void cmTrayActualInformation_Click(object sender, EventArgs e) {
            this.cmTrayActualInformation.Checked = !this.cmTrayActualInformation.Checked;
            if(this.cmTrayActualInformation.Checked && this.cmTrayClearWriter.Checked)
                this.cmTrayClearWriter.Checked = false;
        }

        /// <summary>
        /// Informace o ukon�en� v�po�tu
        /// </summary>
        private void cmTrayFinishInformation_Click(object sender, EventArgs e) {
            this.cmTrayFinishInformation.Checked = !this.cmTrayFinishInformation.Checked;
        }

        /// <summary>
        /// Informace pouze p�i vymaz�n� writeru
        /// </summary>
        private void cmTrayClearWriter_Click(object sender, EventArgs e) {
            this.cmTrayClearWriter.Checked = !this.cmTrayClearWriter.Checked;
            if(this.cmTrayClearWriter.Checked && this.cmTrayActualInformation.Checked)
                this.cmTrayActualInformation.Checked = false;
        }

        #endregion

        private const string messageFailedOpen = "Otev�en� souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}";
        private const string messageFailedOpenDetail = "Otev�en� souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedOpen = "Chyba!";

        private const string errorMessageEditorNotFound = "K formul��i {0} nebyl nalezen rodi�ovsk� editor!";

        private const string registryKeyPositionX = "PositionX";
        private const string registryKeyPositionY = "PositionY";
        private const string registryKeyWidth = "Width";
        private const string registryKeyHeight = "Height";
        private const string registryKeyOpenedFile = "OpenedFile{0}";

        private const string registryKeyClearWriterInformation = "ClearRegistryInfo";
        private const string registryKeyActualInformation = "ActualInfo";
        private const string registryKeyFinishedInformation = "FinishedInfo";

        private const int numBalloonLines = 3;
    }
}