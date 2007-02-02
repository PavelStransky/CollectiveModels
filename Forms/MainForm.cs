using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    public partial class MainForm : Form {
        private ArrayList openedFileNames = new ArrayList();

        /// <summary>
        /// Otevøené soubory pøi ukonèování aplikace
        /// </summary>
        public ArrayList OpenedFileNames { get { return this.openedFileNames; } }

        /// <summary>
        /// Vrací èi nastavuje aktuální adresáø
        /// </summary>
        public string Directory {
            get {
                if(openFileDialog.FileName == string.Empty)
                    return openFileDialog.InitialDirectory;
                else
                    return Path.GetDirectoryName(openFileDialog.FileName);
            }
            set { this.openFileDialog.InitialDirectory = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainForm() {
            this.InitializeComponent();
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
            this.InitializeComponent();
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
            this.SetDialogProperties(this.openFileDialog);

            if(this.IsRegistered)
                this.mnSetttingsRegistry.Checked = true;
            else
                this.mnSetttingsRegistry.Checked = false;

            this.Text = Application.ProductName;

            // Naètení záznamù z registrù
            RegistryKey rk = Application.UserAppDataRegistry;
            object x = rk.GetValue(registryKeyPositionX);
            object y = rk.GetValue(registryKeyPositionY);
            if(x is int && y is int && (int)x > 0 && (int)y > 0) {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point((int)x, (int)y);
            }
            else
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;

            object width = rk.GetValue(registryKeyWidth);
            object height = rk.GetValue(registryKeyHeight);

            if(width is int && height is int && (int)width > 0 && (int)height > 0)
                this.Size = new Size((int)width, (int)height);

            object directory = rk.GetValue(registryKeyDirectory);
            if(directory is string && directory as string != string.Empty)
                this.Directory = directory as string;

            object fncDirectory = rk.GetValue(registryKeyFncDirectory);
            if(fncDirectory is string && fncDirectory as string != string.Empty)
                Context.FncDirectory = fncDirectory as string;

            int i = 0;
            object openedFile;
            while((openedFile = rk.GetValue(string.Format(registryKeyOpenedFile, i))) != null) {
                this.openedFileNames.Add(openedFile);
                rk.DeleteValue(string.Format(registryKeyOpenedFile, i));
                i++;
            }
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
            editor.Directory = this.Directory;
            editor.MdiParent = this;
            editor.Show();
            editor.FormClosed += new FormClosedEventHandler(this.editor_FormClosed);
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
        /// Otevøe soubor
        /// </summary>
        /// <param name="fileName">Název souboru</param>
        private void Open(string fileName) {
            Import import = null;
            Editor editor = null;

            // Pøídání pøípony
            if(fileName.Length < 3 || fileName.Substring(fileName.Length - 3, 3) != WinMain.FileExtGcm)
                fileName = string.Format("{0}.{1}", fileName, WinMain.FileExtGcm);

            try {
                import = new Import(fileName, true);
                editor = import.Read() as Editor;

                editor.FormClosed += new FormClosedEventHandler(this.editor_FormClosed);
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
                this.mnFileSeparator1.Visible = true;
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

            RegistryKey rk = Application.UserAppDataRegistry;
            rk.SetValue(registryKeyPositionX, this.Location.X);
            rk.SetValue(registryKeyPositionY, this.Location.Y);
            rk.SetValue(registryKeyWidth, this.Width);
            rk.SetValue(registryKeyHeight, this.Height);
            rk.SetValue(registryKeyDirectory, this.Directory);
            rk.SetValue(registryKeyFncDirectory, Context.FncDirectory);

            int i = 0;
            foreach(string fileName in this.openedFileNames)
                rk.SetValue(string.Format(registryKeyOpenedFile, i++), fileName);
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
        /// Vytvoøí název aplikace pro registr Windows
        /// </summary>
        public string RegistryEntryName {
            get {
                string companyName = Application.CompanyName.Trim().Replace(" ", string.Empty);
                string productName = Application.ProductName.Trim().Replace(" ", string.Empty);
                string version = Application.ProductVersion; version = version.Substring(0, version.IndexOf('.'));
                return string.Format("{0}.{1}.{2}", companyName, productName, version);
            }
        }

        /// <summary>
        /// Je program zaregistrován?
        /// </summary>
        private bool IsRegistered {
            get {
                string path = Application.ExecutablePath;
                string keyName = this.RegistryEntryName;

                // Existuje klíè v registrech?
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(string.Format(commandEntryName, keyName));
                if(rk == null)
                    return false;
                
                // Existuje záznam s cestou?
                string commandEntry = rk.GetValue(string.Empty) as string;
                if(commandEntry == null || commandEntry == string.Empty)
                    return false;

                if(string.Format(commandEntryFormat, path) != commandEntry)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Registrace pøípony
        /// </summary>
        private void mnSetttingsRegistry_Click(object sender, EventArgs e) {
            string path = Application.ExecutablePath;
            string keyName = this.RegistryEntryName;

            // Podle stavu buï zaregistrujeme, nebo odregistrujeme
            if(this.mnSetttingsRegistry.Checked) {
                try {
                    Registry.ClassesRoot.DeleteSubKeyTree(keyName);
                    Registry.ClassesRoot.DeleteSubKeyTree('.' + WinMain.FileExtGcm);
                }
                catch(Exception) {
                }

                this.mnSetttingsRegistry.Checked = false;
            }
            else {
                Registry.ClassesRoot.CreateSubKey('.' + WinMain.FileExtGcm).SetValue(string.Empty, keyName);
                Registry.ClassesRoot.CreateSubKey(keyName).SetValue(string.Empty, programDescription);
                Registry.ClassesRoot.CreateSubKey(string.Format("{0}\\DefaultIcon", keyName)).SetValue(string.Empty, string.Format("{0},0", path));
                Registry.ClassesRoot.CreateSubKey(string.Format(commandEntryName, keyName)).SetValue(string.Empty, string.Format(commandEntryFormat, path));

                this.mnSetttingsRegistry.Checked = true;
            }
        }
        #endregion

        private const string commandEntryName = "{0}\\shell\\open\\command";
        private const string commandEntryFormat = "\"{0}\" \"%1\"";
        private const string programDescription = "Program for analysing nuclear collective models (GCM, IBM)";

        private const string messageFailedOpen = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}";
        private const string messageFailedOpenDetail = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedOpen = "Chyba!";

        private const string errorMessageEditorNotFound = "K formuláøi {0} nebyl nalezen rodièovský editor!";

        private const string registryKeyPositionX = "PositionX";
        private const string registryKeyPositionY = "PositionY";
        private const string registryKeyWidth = "Width";
        private const string registryKeyHeight = "Height";
        private const string registryKeyOpenedFile = "OpenedFile{0}";
        private const string registryKeyDirectory = "Directory";
        private const string registryKeyFncDirectory = "FncDirectory";
    }
}