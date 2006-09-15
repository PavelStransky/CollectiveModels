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
        /// Dialog k uložení 
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
        /// Konstruktor s otevøením souboru
        /// </summary>
        /// <param name="fName">Název souboru</param>
        public MainForm(string fName) {
            this.InitializeComponent();

            this.SetDialogProperties(this.openFileDialog);
            this.SetDialogProperties(this.saveFileDialog);

            this.NewEditor().Open(fName);
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

        private const string defaultFileFilter = "Soubory historie (*.gcm)|*.gcm|Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";
        private const string defaultFileExt = "his";
        private const string defaultDirectory = "c:\\gcm";

    }
}