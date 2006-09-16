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
        /// Dialog k ulo�en� 
        /// </summary>
        public SaveFileDialog SaveFileDialog { get { return this.saveFileDialog; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainForm() {
            this.InitializeComponent();
            this.Initialize();
            this.New();
        }

        /// <summary>
        /// Konstruktor s otev�en�m souboru
        /// </summary>
        /// <param name="fName">N�zev souboru</param>
        public MainForm(string fName) {
            this.InitializeComponent();
            this.Initialize();
            this.Open(fName);
        }

        /// <summary>
        /// Inicializace instance nov� GCM
        /// </summary>
        private void Initialize() {
            this.SetDialogProperties(this.openFileDialog);
            this.SetDialogProperties(this.saveFileDialog);

            if(this.IsRegistered)
                this.mnSetttingsRegistry.Checked = true;
            else
                this.mnSetttingsRegistry.Checked = false;

            this.Text = Application.ProductName;
        }

        /// <summary>
        /// Nastav� vlastnosti dialogu
        /// </summary>
        /// <param name="dialog">Dialog</param>
        private void SetDialogProperties(FileDialog dialog) {
            dialog.Reset();
            dialog.Filter = defaultFileFilter;
            dialog.DefaultExt = defaultFileExt;
            dialog.InitialDirectory = defaultDirectory;
        }

        /// <summary>
        /// Vytvo�� nev� okno s editorem
        /// </summary>
        private void New() {
            Editor editor = new Editor();
            editor.MdiParent = this;
            editor.Show();
        }

        /// <summary>
        /// Vytvo�� nov� formul��
        /// </summary>
        /// <param name="type">Typ formul��e</param>
        /// <param name="parent">Ke kter�mu formul��i (editoru) pat��</param>
        /// <param name="name">N�zev okna</param>
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
        /// Otev��t
        /// </summary>
        private void mnFileOpen_Click(object sender, EventArgs e) {
            this.openFileDialog.ShowDialog();
        }

        /// <summary>
        /// Ulo�it
        /// </summary>
        private void mnFileSave_Click(object sender, EventArgs e) {
            if(this.ActiveMdiChild is Editor) {
                string fileName = (this.ActiveMdiChild as Editor).FileName;
                if(fileName == null || fileName == string.Empty)
                    this.saveFileDialog.ShowDialog();
                else
                    this.Save(this.ActiveMdiChild as Editor, fileName);
            }
        }

        /// <summary>
        /// Ulo�it jako
        /// </summary>
        private void mnFileSaveAs_Click(object sender, EventArgs e) {
            if(this.ActiveMdiChild is Editor) {
                this.saveFileDialog.FileName = (this.ActiveMdiChild as Editor).FileName;
                this.saveFileDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Ukon�� aplikaci
        /// </summary>
        private void mnExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        /// <summary>
        /// Otev�en� souboru - vol�no z dialogu FileOpen
        /// </summary>
        private void openFileDialog_FileOk(object sender, CancelEventArgs e) {
            this.Open(this.openFileDialog.FileName);
        }

        /// <summary>
        /// Ulo�en� souboru - vol�no z dialogu FileSave
        /// </summary>
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e) {
            if(this.ActiveMdiChild is Editor)
                this.Save(this.ActiveMdiChild as Editor, this.saveFileDialog.FileName);
        }

        /// <summary>
        /// Otev�e soubor
        /// </summary>
        /// <param name="fileName">N�zev souboru</param>
        private void Open(string fileName) {
            Import import = new Import(fileName, true);
            Editor editor = import.Read() as Editor;

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
                    // P�epo��t�me graf
                    string expressionText = editor.Context[(o as GraphForm).Name].Expression + ";";
                    Expression.Expression expression = new PavelStransky.Expression.Expression(editor.Context, expressionText);
                    expression.Evaluate();
                }
            }

            import.Close();

            editor.Modified = false;
            editor.Activate();
        }

        /// <summary>
        /// Ulo�� soubor
        /// </summary>
        /// <param name="editor">Okno editoru</param>
        /// <param name="fileName">N�zev souboru</param>
        private void Save(Editor editor, string fileName) {
            editor.FileName = fileName;

            Export export = new Export(fileName, true);
            export.Write(editor);

            int num = 0;
            for(int i = 0; i < this.MdiChildren.Length; i++) {
                ChildForm childForm = this.MdiChildren[i] as ChildForm;
                if(childForm as IExportable != null && childForm.ParentEditor == editor)
                    num++;
            }

            export.B.Write(num);
            for(int i = 0; i < this.MdiChildren.Length; i++) {
                ChildForm childForm = this.MdiChildren[i] as ChildForm;
                if(childForm as IExportable != null && childForm.ParentEditor == editor)
                    export.Write(childForm);
            }

            export.Close();

            editor.Modified = false;
        }
        #endregion

        #region Menu Nastaven�
        /// <summary>
        /// Vytvo�� n�zev aplikace pro registr Windows
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

        private const string defaultFileFilter = "Soubory historie (*.gcm)|*.gcm|Textov� soubory (*.txt)|*.txt|V�echny soubory (*.*)|*.*";
        private const string defaultFileExt = "gcm";
        private const string defaultDirectory = "c:\\gcm";

        private const string commandEntryName = "{0}\\shell\\open\\command";
        private const string commandEntryFormat = "\"{0}\" %1";
        private const string programDescription = "Program for analysing nuclear collective models (GCM, IBM)";

        private const int margin = 8;
    }
}