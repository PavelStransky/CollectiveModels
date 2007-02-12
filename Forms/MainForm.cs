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
        /// Otevøené soubory pøi ukonèování aplikace
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
        /// Konstruktor s otevøením souboru
        /// </summary>
        /// <param name="fileName">Název souboru</param>
        public MainForm(string fileName) {
            this.Initialize();
            this.Show();

            this.Open(fileName);

            this.SetMenu();
            this.openedFileNames.Clear();
        }

        /// <summary>
        /// Inicializace instance nové GCM
        /// </summary>
        private void Initialize() {
            this.InitializeComponent();

            this.Menu = this.mnMenu;
            this.SetDialogProperties(this.openFileDialog);

            this.mnSetttingsRegistry.Checked = WinMain.IsRegistered;
            this.mnSettingsPlaySounds.Checked = WinMain.PlaySounds;

            this.Text = Application.ProductName;

            // Naètení záznamù z registrù
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
        /// Nastaví vlastnosti dialogu
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
        /// Seøadit okna
        /// </summary>
        private void MnCascade_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        /// <summary>
        /// Dlaždice horizontálnì
        /// </summary>
        private void mnTileHorizontal_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        /// <summary>
        /// Dlaždice vertikálnì
        /// </summary>
        private void mnTileVertical_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        /// <summary>
        /// Seøadit ikony
        /// </summary>
        private void mnArrangeIcons_Click(object sender, EventArgs e) {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }
        #endregion

        #region Menu Soubor
        /// <summary>
        /// Nové okno
        /// </summary>
        private void mnFileNew_Click(object sender, EventArgs e) {
            this.New();
        }

        /// <summary>
        /// Vytvoøí nevé okno s editorem
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
        /// Otevøít
        /// </summary>
        private void mnFileOpen_Click(object sender, EventArgs e) {
            this.openFileDialog.ShowDialog();
        }

        /// <summary>
        /// Otevøení souboru - voláno z dialogu FileOpen
        /// </summary>
        private void openFileDialog_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetDirectoryFromFile(this.openFileDialog.FileName);
            this.Open(this.openFileDialog.FileName);
            this.SetMenu();
        }

        /// <summary>
        /// Pøi vybrání položky ze seznamu naposledy otevøených souborù
        /// </summary>
        void lastOpenedFiles_Click(object sender, FileNameEventArgs e) {
            WinMain.SetDirectoryFromFile(e.FileName);
            this.Open(e.FileName);
            this.SetMenu();
        }

        /// <summary>
        /// Otevøe soubor
        /// </summary>
        /// <param name="fileName">Název souboru</param>
        private void Open(string fileName) {
            Import import = null;
            Editor editor = null;

            // Pøídání pøípony
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

                // Pokud máme správnou verzi, naèteme ostatní okna
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
        /// Voláno po úspìšném otevøení souboru
        /// </summary>
        protected virtual void OnFileOpened(FileNameEventArgs e) {
            if(this.FileOpened != null)
                this.FileOpened(this, e);
        }

        /// <summary>
        /// Najde editor pøíslušející k aktivnímu oknu
        /// </summary>
        /// <param name="form">Aktivní okno</param>
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
        /// Nastaví menu podle stavu otevøených oken
        /// </summary>
        public void SetMenu() {
            this.SetMenu(null);
        }

        /// <summary>
        /// Nastaví menu podle stavu otevøených oken
        /// </summary>
        /// <param name="closed">Právì uzavírané okno (již uvažujeme, že je uzavøené)</param>
        private void SetMenu(Editor closed) {
            bool isEditor = false;

            // Hledáme otevøený editor
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
        /// Uložit
        /// </summary>
        private void mnFileSave_Click(object sender, EventArgs e) {
            if(!this.mnFileSave.Visible)
                return;
            Editor editor = this.FindActiveMdiEditor(this.ActiveMdiChild);
            editor.Save();
        }

        /// <summary>
        /// Uložit jako
        /// </summary>
        private void mnFileSaveAs_Click(object sender, EventArgs e) {
            Editor editor = this.FindActiveMdiEditor(this.ActiveMdiChild);
            editor.SaveAs();
        }

        /// <summary>
        /// Zavøít
        /// </summary>
        private void mnFileClose_Click(object sender, EventArgs e) {
            Editor editor = this.FindActiveMdiEditor(this.ActiveMdiChild);
            editor.Close();
            this.SetMenu();
        }

        /// <summary>
        /// Ukonèí aplikaci
        /// </summary>
        private void mnExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// Pøi ukonèení ukládáme informace o aktuálním oknì
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
        /// Uzavøení editoru - pøípadné skrytí menu
        /// </summary>
        void editor_FormClosed(object sender, FormClosedEventArgs e) {
            this.SetMenu(sender as Editor);
        }

        #region Menu Nastavení
        /// <summary>
        /// Registrace pøípony
        /// </summary>
        private void mnSetttingsRegistry_Click(object sender, EventArgs e) {
            this.mnSetttingsRegistry.Checked = !this.mnSetttingsRegistry.Checked;
            WinMain.Register(this.mnSetttingsRegistry.Checked);
        }

        /// <summary>
        /// Pøehrávat zvuky
        /// </summary>
        private void mnSettingsPlaySounds_Click(object sender, EventArgs e) {
            this.mnSettingsPlaySounds.Checked = !this.mnSettingsPlaySounds.Checked;
            WinMain.PlaySounds = this.mnSettingsPlaySounds.Checked;
        }
        #endregion

        #region Menu Skrýt
        // Priorita pøed minimalizováním
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

            // Text mùže mít maximálnì 64 znakù
            this.trayIcon.Text = text.Substring(0, System.Math.Min(text.Length, 64));

            foreach(Form rf in this.MdiChildren) 
                if(rf as ResultForm != null) {
                    if((rf as ResultForm).Calculating) {
                        (rf as ResultForm).CalcFinished += this.rf_CalcFinished;
                        (rf as ResultForm).TxtResult.TextChanged += new EventHandler(TxtResult_TextChanged);
                    }
                }

            // Nastavení priority
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
                this.trayIcon.ShowBalloonTip(5000, "Probíhá výpoèet...", text, ToolTipIcon.Info);
        }

        // Ukonèení výpoètu
        void rf_CalcFinished(object sender, FinishedEventArgs e) {
            this.balloonText = string.Empty;
            ResultForm rf = sender as ResultForm;

            if(rf == null || !this.trayIcon.Visible)
                return;

            if(this.cmTrayFinishInformation.Checked)
                this.trayIcon.ShowBalloonTip(10000, "Dokonèen výpoèet", rf.TxtCommand.Text, ToolTipIcon.Info);
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e) {
            // Zachytáváme pouze stisk levého tlaèítka myši
            if(e.Button != MouseButtons.Left)
                return;

            this.trayIcon.Visible = false;
            this.Show();

            // Nastavení priority
            Process process = new Process();
            if(process.GetPriority() == ProcessPriority.Idle)
                process.SetPriority(this.priority);

            foreach(Form rf in this.MdiChildren)
                if(rf as ResultForm != null) 
                    (rf as ResultForm).CalcFinished -= this.rf_CalcFinished;                
        }

        /// <summary>
        /// Aktuální informace
        /// </summary>
        private void cmTrayActualInformation_Click(object sender, EventArgs e) {
            this.cmTrayActualInformation.Checked = !this.cmTrayActualInformation.Checked;
            if(this.cmTrayActualInformation.Checked && this.cmTrayClearWriter.Checked)
                this.cmTrayClearWriter.Checked = false;
        }

        /// <summary>
        /// Informace o ukonèení výpoètu
        /// </summary>
        private void cmTrayFinishInformation_Click(object sender, EventArgs e) {
            this.cmTrayFinishInformation.Checked = !this.cmTrayFinishInformation.Checked;
        }

        /// <summary>
        /// Informace pouze pøi vymazání writeru
        /// </summary>
        private void cmTrayClearWriter_Click(object sender, EventArgs e) {
            this.cmTrayClearWriter.Checked = !this.cmTrayClearWriter.Checked;
            if(this.cmTrayClearWriter.Checked && this.cmTrayActualInformation.Checked)
                this.cmTrayActualInformation.Checked = false;
        }

        #endregion

        private const string messageFailedOpen = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}";
        private const string messageFailedOpenDetail = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedOpen = "Chyba!";

        private const string errorMessageEditorNotFound = "K formuláøi {0} nebyl nalezen rodièovský editor!";

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