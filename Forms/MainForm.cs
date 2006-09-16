using System;
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
        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainForm() {
            this.InitializeComponent();
            this.Initialize();
            this.New();
        }

        /// <summary>
        /// Konstruktor s otevøením souboru
        /// </summary>
        /// <param name="fName">Název souboru</param>
        public MainForm(string fName) {
            this.InitializeComponent();
            this.Initialize();
            this.Open(fName);
        }

        /// <summary>
        /// Inicializace instance nové GCM
        /// </summary>
        private void Initialize() {
            this.SetDialogProperties(this.openFileDialog, defaultDirectory);

            if(this.IsRegistered)
                this.mnSetttingsRegistry.Checked = true;
            else
                this.mnSetttingsRegistry.Checked = false;

            this.Text = Application.ProductName;
        }

        /// <summary>
        /// Nastaví vlastnosti dialogu
        /// </summary>
        /// <param name="dialog">Dialog</param>
        private void SetDialogProperties(FileDialog dialog, string directory) {
            dialog.Reset();
            dialog.Filter = defaultFileFilter;
            dialog.DefaultExt = defaultFileExt;
            dialog.InitialDirectory = directory;
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
            this.SetDialogProperties(editor.SaveFileDialog, this.openFileDialog.InitialDirectory);
            editor.MdiParent = this;
            editor.Show();
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
            this.Open(this.openFileDialog.FileName);
        }

        /// <summary>
        /// Otevøe soubor
        /// </summary>
        /// <param name="fileName">Název souboru</param>
        private void Open(string fileName) {
            Import import = null;
            Editor editor = null;

            try {
                import = new Import(fileName, true);
                editor = import.Read() as Editor;

                editor.MdiParent = this;
                editor.Show();

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
                    if(o is GraphForm) {
                        // Pøepoèítáme graf
                        string expressionText = editor.Context[(o as GraphForm).Name].Expression + ";";
                        Expression.Expression expression = new PavelStransky.Expression.Expression(editor.Context, expressionText);
                        expression.Evaluate();
                    }
                }
            }
            catch(DetailException e) {
                MessageBox.Show(this, string.Format(messageFailedOpenDetail, fileName, e.Message, e.DetailMessage),
                    captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            catch(Exception e) {
                MessageBox.Show(this, string.Format(messageFailedOpen, fileName, e.Message),
                    captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally {
                import.Close();
            }

            editor.FileName = fileName;
            editor.Modified = false;
            editor.Activate();
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
        }

        /// <summary>
        /// Ukonèí aplikaci
        /// </summary>
        private void mnExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }
        #endregion

        #region Menu Nastavení
        /// <summary>
        /// Vytvoøí název aplikace pro registr Windows
        /// </summary>
        private string RegistryEntryName {
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
                Registry.ClassesRoot.DeleteSubKeyTree('.' + defaultFileExt);
                Registry.ClassesRoot.DeleteSubKeyTree(keyName);

                this.mnSetttingsRegistry.Checked = false;
            }
            else {
                Registry.ClassesRoot.CreateSubKey('.' + defaultFileExt).SetValue(string.Empty, keyName);
                Registry.ClassesRoot.CreateSubKey(keyName).SetValue(string.Empty, programDescription);
                Registry.ClassesRoot.CreateSubKey(string.Format("{0}\\DefaultIcon", keyName)).SetValue(string.Empty, string.Format("{0},0", path));
                Registry.ClassesRoot.CreateSubKey(string.Format(commandEntryName, keyName)).SetValue(string.Empty, string.Format(commandEntryFormat, path));

                this.mnSetttingsRegistry.Checked = true;
            }
        }
        #endregion

        private const string defaultFileFilter = "Soubory historie (*.gcm)|*.gcm|Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";
        private const string defaultFileExt = "gcm";
        private const string defaultDirectory = "c:\\gcm";

        private const string commandEntryName = "{0}\\shell\\open\\command";
        private const string commandEntryFormat = "\"{0}\" %1";
        private const string programDescription = "Program for analysing nuclear collective models (GCM, IBM)";

        private const string messageFailedOpen = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}";
        private const string messageFailedOpenDetail = "Otevøení souboru '{0}' se nezdaøilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedOpen = "Chyba!";

        private const string errorMessageEditorNotFound = "K formuláøi {0} nebyl nalezen rodièovský editor!";
    }
}