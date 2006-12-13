namespace PavelStransky.Forms {
    partial class GraphControl {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.cmnSaveAsTxt = new System.Windows.Forms.MenuItem();
            this.cmnSaveAsGif = new System.Windows.Forms.MenuItem();
            this.saveFileDialogTxt = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialogGif = new System.Windows.Forms.SaveFileDialog();
            this.cmnSaveAsPicture = new System.Windows.Forms.MenuItem();
            this.saveFileDialogPicture = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(150, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cmnSaveAsTxt,
            this.cmnSaveAsGif,
            this.cmnSaveAsPicture});
            // 
            // cmnSaveAsTxt
            // 
            this.cmnSaveAsTxt.Index = 0;
            this.cmnSaveAsTxt.Text = "Uložit jako &text...";
            this.cmnSaveAsTxt.Click += new System.EventHandler(this.cmnSaveAsTxt_Click);
            // 
            // cmnSaveAsGif
            // 
            this.cmnSaveAsGif.Index = 1;
            this.cmnSaveAsGif.Text = "Uložit jako &GIF (animovaný)...";
            this.cmnSaveAsGif.Click += new System.EventHandler(this.cmnSaveAsGif_Click);
            // 
            // saveFileDialogTxt
            // 
            this.saveFileDialogTxt.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogTxt_FileOk);
            // 
            // saveFileDialogGif
            // 
            this.saveFileDialogGif.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogGif_FileOk);
            // 
            // cmnSaveAsPicture
            // 
            this.cmnSaveAsPicture.Index = 2;
            this.cmnSaveAsPicture.Text = "Uložit jako obrázek (sekvenènì)...";
            this.cmnSaveAsPicture.Click += new System.EventHandler(this.cmnSaveAsPicture_Click);
            // 
            // saveFileDialogPicture
            // 
            this.saveFileDialogPicture.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogPicture_FileOk);
            // 
            // GraphControl
            // 
            this.ContextMenu = this.contextMenu;
            this.Controls.Add(this.lblTitle);
            this.Name = "GraphControl";
            this.Size = new System.Drawing.Size(150, 190);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem cmnSaveAsTxt;
        private System.Windows.Forms.SaveFileDialog saveFileDialogTxt;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SaveFileDialog saveFileDialogGif;
        private System.Windows.Forms.MenuItem cmnSaveAsGif;
        private System.Windows.Forms.MenuItem cmnSaveAsPicture;
        private System.Windows.Forms.SaveFileDialog saveFileDialogPicture;
    }
}
