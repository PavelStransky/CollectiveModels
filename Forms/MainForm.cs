using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

namespace PavelStransky.Forms {
    public partial class MainForm : Form {
        /// <summary>
        /// Dialog k uložení 
        /// </summary>
        public SaveFileDialog SaveFileDialog { get { return this.saveFileDialog; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainForm() {
            this.InitializeComponent();
            this.Initialize();
            this.NewEditor();
        }

        /// <summary>
        /// Konstruktor s otevøením souboru
        /// </summary>
        /// <param name="fName">Název souboru</param>
        public MainForm(string fName) {
            this.InitializeComponent();
            this.Initialize();
            this.NewEditor().Open(fName);
        }

        /// <summary>
        /// Inicializace instance nové GCM
        /// </summary>
        private void Initialize() {
            this.SetDialogProperties(this.openFileDialog);
            this.SetDialogProperties(this.saveFileDialog);

            if(this.IsRegistered)
                this.mnSetttingsRegistry.Checked = true;
            else
                this.mnSetttingsRegistry.Checked = false;
        }

        /// <summary>
        /// Nastaví vlastnosti dialogu
        /// </summary>
        /// <param name="dialog">Dialog</param>
        private void SetDialogProperties(FileDialog dialog) {
            dialog.Reset();
            dialog.Filter = defaultFileFilter;
            dialog.DefaultExt = defaultFileExt;
            dialog.InitialDirectory = defaultDirectory;
        }

        /// <summary>
        /// Vytvoøí nevé okno s editorem
        /// </summary>
        private Editor NewEditor() {
            Editor editor = new Editor();
            editor.MdiParent = this;
            editor.Show();

            return editor;
        }

        /// <summary>
        /// Vytvoøí nový formuláø
        /// </summary>
        /// <param name="type">Typ formuláøe</param>
        /// <param name="parent">Ke kterému formuláøi (editoru) patøí</param>
        /// <param name="name">Název okna</param>
        public ChildForm NewParentForm(Type type, Editor parent, string name) {
            ChildForm result;

            for(int i = 0; i < this.MdiChildren.Length; i++) {
                result = this.MdiChildren[i] as ChildForm;

                if(result != null && result.ParentEditor == parent && result.Name == name && result.GetType() == type)
                    return result;
            }

            if(type == typeof(GraphForm)) {
                result = new GraphForm();
                result.Location = new Point(margin, this.Height - result.Size.Height - 8 * margin);
            }
            else if(type == typeof(ResultForm)) {
                result = new ResultForm();
                result.Location = new Point(this.Width - result.Size.Width - 2 * margin, margin);
            }
            else
                result = new ChildForm();

            result.Name = name;
            result.Text = name;
            result.ParentEditor = parent;
            result.MdiParent = this;
            return result;
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
            this.NewEditor();
        }

        /// <summary>
        /// Otevøít
        /// </summary>
        private void mnFileOpen_Click(object sender, EventArgs e) {
            this.openFileDialog.ShowDialog();
        }

        /// <summary>
        /// Uložit
        /// </summary>
        private void mnFileSave_Click(object sender, EventArgs e) {
            if(this.ActiveMdiChild is Editor)
                (this.ActiveMdiChild as Editor).Save();
        }

        /// <summary>
        /// Uložit jako
        /// </summary>
        private void mnFileSaveAs_Click(object sender, EventArgs e) {
            if(this.ActiveMdiChild is Editor) {
                this.saveFileDialog.FileName = (this.ActiveMdiChild as Editor).FileName;
                this.saveFileDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Otevøení souboru - voláno z dialogu FileOpen
        /// </summary>
        private void openFileDialog_FileOk(object sender, CancelEventArgs e) {
            this.NewEditor().Open(this.openFileDialog.FileName);
        }

        /// <summary>
        /// Uložení souboru - voláno z dialogu FileSave
        /// </summary>
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e) {
            if(this.ActiveMdiChild is Editor)
                (this.ActiveMdiChild as Editor).Save(this.saveFileDialog.FileName);
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

        private const int margin = 8;
    }
}