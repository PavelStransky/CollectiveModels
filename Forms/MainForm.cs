using System;
using System.Collections;
using System.Collections.Generic;
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
        /// Otev�en� soubory p�i ukon�ov�n� aplikace
        /// </summary>
        public ArrayList OpenedFileNames { get { return this.openedFileNames; } }

        /// <summary>
        /// Vrac� �i nastavuje aktu�ln� adres��
        /// </summary>
        public string Directory { get { return this.openFileDialog.InitialDirectory; } set { this.openFileDialog.InitialDirectory = value; } }

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

            this.openedFileNames.Clear();
        }

        /// <summary>
        /// Konstruktor s otev�en�m souboru
        /// </summary>
        /// <param name="fileName">N�zev souboru</param>
        public MainForm(string fileName) {
            this.InitializeComponent();
            this.Initialize();
            this.Show();
            this.Open(fileName);

            this.openedFileNames.Clear();
        }

        /// <summary>
        /// Inicializace instance nov� GCM
        /// </summary>
        private void Initialize() {
            this.SetDialogProperties(this.openFileDialog);

            if(this.IsRegistered)
                this.mnSetttingsRegistry.Checked = true;
            else
                this.mnSetttingsRegistry.Checked = false;

            this.Text = Application.ProductName;

            // Na�ten� z�znam� z registr�
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

            int i = 0;
            object openedFile;
            while((openedFile = rk.GetValue(string.Format(registryKeyOpenedFile, i))) != null) {
                this.openedFileNames.Add(openedFile);
                rk.DeleteValue(string.Format(registryKeyOpenedFile, i));
                i++;
            }
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
            editor.MdiParent = this;
            editor.Show();
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
        }

        /// <summary>
        /// Otev�e soubor
        /// </summary>
        /// <param name="fileName">N�zev souboru</param>
        private void Open(string fileName) {
            Import import = null;
            Editor editor = null;

            // P��d�n� p��pony
            if(fileName.Length < 3 || fileName.Substring(fileName.Length - 3, 3) != WinMain.FileExtGcm)
                fileName = string.Format("{0}.{1}", fileName, WinMain.FileExtGcm);

            try {
                import = new Import(fileName, true);
                editor = import.Read() as Editor;

                editor.MdiParent = this;
                editor.Show();

                // Pokud m�me spr�vnou verzi, na�teme ostatn� okna
                if(import.VersionNumber >= 1) {
                    int num = import.B.ReadInt32();
                    for(int i = 0; i < num; i++) {
                        object o = import.Read();
                        if(o is ChildForm) {
                            (o as ChildForm).ParentEditor = editor;
                            (o as ChildForm).MdiParent = this;
                            (o as ChildForm).Show();
                        }
                        if(o is ResultForm) {
                            (o as ResultForm).SetContext(editor.Context);
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
            finally {
                import.Close();
            }
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
        /// Ulo�it
        /// </summary>
        private void mnFileSave_Click(object sender, EventArgs e) {
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

            RegistryKey rk = Application.UserAppDataRegistry;
            rk.SetValue(registryKeyPositionX, this.Location.X);
            rk.SetValue(registryKeyPositionY, this.Location.Y);
            rk.SetValue(registryKeyWidth, this.Width);
            rk.SetValue(registryKeyHeight, this.Height);

            int i = 0;
            foreach(string fileName in this.openedFileNames)
                rk.SetValue(string.Format(registryKeyOpenedFile, i++), fileName);
        }
        #endregion

        #region Menu Nastaven�
        /// <summary>
        /// Vytvo�� n�zev aplikace pro registr Windows
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
        /// Je program zaregistrov�n?
        /// </summary>
        private bool IsRegistered {
            get {
                string path = Application.ExecutablePath;
                string keyName = this.RegistryEntryName;

                // Existuje kl�� v registrech?
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(string.Format(commandEntryName, keyName));
                if(rk == null)
                    return false;
                
                // Existuje z�znam s cestou?
                string commandEntry = rk.GetValue(string.Empty) as string;
                if(commandEntry == null || commandEntry == string.Empty)
                    return false;

                if(string.Format(commandEntryFormat, path) != commandEntry)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Registrace p��pony
        /// </summary>
        private void mnSetttingsRegistry_Click(object sender, EventArgs e) {
            string path = Application.ExecutablePath;
            string keyName = this.RegistryEntryName;

            // Podle stavu bu� zaregistrujeme, nebo odregistrujeme
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

        private const string messageFailedOpen = "Otev�en� souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}";
        private const string messageFailedOpenDetail = "Otev�en� souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedOpen = "Chyba!";

        private const string errorMessageEditorNotFound = "K formul��i {0} nebyl nalezen rodi�ovsk� editor!";

        private const string registryKeyPositionX = "PositionX";
        private const string registryKeyPositionY = "PositionY";
        private const string registryKeyWidth = "Width";
        private const string registryKeyHeight = "Height";
        private const string registryKeyOpenedFile = "OpenedFile{0}";
    }
}