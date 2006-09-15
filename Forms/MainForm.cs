using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

            this.SetDialogProperties(this.openFileDialog);
            this.SetDialogProperties(this.saveFileDialog);

            this.NewEditor();
        }

        /// <summary>
        /// Konstruktor s otev�en�m souboru
        /// </summary>
        /// <param name="fName">N�zev souboru</param>
        public MainForm(string fName) {
            this.InitializeComponent();

            this.SetDialogProperties(this.openFileDialog);
            this.SetDialogProperties(this.saveFileDialog);

            this.NewEditor().Open(fName);
        }

        /// <summary>
        /// Vytvo�� nev� okno s editorem
        /// </summary>
        private Editor NewEditor() {
            Editor editor = new Editor();
            editor.MdiParent = this;
            editor.Show();

            return editor;
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

            if(type == typeof(GraphForm))
                result = new GraphForm();
            else if(type == typeof(ResultForm))
                result = new ResultForm();
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
            this.NewEditor();
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
            if(this.ActiveMdiChild is Editor)
                (this.ActiveMdiChild as Editor).Save();
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
        /// Otev�en� souboru - vol�no z dialogu FileOpen
        /// </summary>
        private void openFileDialog_FileOk(object sender, CancelEventArgs e) {
            this.NewEditor().Open(this.openFileDialog.FileName);
        }

        /// <summary>
        /// Ulo�en� souboru - vol�no z dialogu FileSave
        /// </summary>
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e) {
            if(this.ActiveMdiChild is Editor)
                (this.ActiveMdiChild as Editor).Save(this.saveFileDialog.FileName);
        }
        #endregion

        private const string defaultFileFilter = "Soubory historie (*.gcm)|*.gcm|Textov� soubory (*.txt)|*.txt|V�echny soubory (*.*)|*.*";
        private const string defaultFileExt = "his";
        private const string defaultDirectory = "c:\\gcm";

    }
}