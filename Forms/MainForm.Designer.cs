namespace PavelStransky.Forms {
    partial class MainForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mnMenu = new System.Windows.Forms.MainMenu(this.components);
            this.mnFile = new System.Windows.Forms.MenuItem();
            this.mnFileNew = new System.Windows.Forms.MenuItem();
            this.mnFileOpen = new System.Windows.Forms.MenuItem();
            this.mnFileClose = new System.Windows.Forms.MenuItem();
            this.mnFileSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnFileSave = new System.Windows.Forms.MenuItem();
            this.mnFileSaveAs = new System.Windows.Forms.MenuItem();
            this.mnFileSeparator2 = new System.Windows.Forms.MenuItem();
            this.mnExit = new System.Windows.Forms.MenuItem();
            this.mnSettings = new System.Windows.Forms.MenuItem();
            this.mnSetttingsRegistry = new System.Windows.Forms.MenuItem();
            this.mnWindow = new System.Windows.Forms.MenuItem();
            this.MnCascade = new System.Windows.Forms.MenuItem();
            this.mnTileHorizontal = new System.Windows.Forms.MenuItem();
            this.mnTileVertical = new System.Windows.Forms.MenuItem();
            this.mnArrangeIcons = new System.Windows.Forms.MenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // mnMenu
            // 
            this.mnMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFile,
            this.mnSettings,
            this.mnWindow});
            // 
            // mnFile
            // 
            this.mnFile.Index = 0;
            this.mnFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFileNew,
            this.mnFileOpen,
            this.mnFileClose,
            this.mnFileSeparator1,
            this.mnFileSave,
            this.mnFileSaveAs,
            this.mnFileSeparator2,
            this.mnExit});
            this.mnFile.Text = "&Soubor";
            // 
            // mnFileNew
            // 
            this.mnFileNew.Index = 0;
            this.mnFileNew.Text = "&Nové";
            this.mnFileNew.Click += new System.EventHandler(this.mnFileNew_Click);
            // 
            // mnFileOpen
            // 
            this.mnFileOpen.Index = 1;
            this.mnFileOpen.Text = "&Otevøít...";
            this.mnFileOpen.Click += new System.EventHandler(this.mnFileOpen_Click);
            // 
            // mnFileClose
            // 
            this.mnFileClose.Index = 2;
            this.mnFileClose.Text = "&Zavøít";
            this.mnFileClose.Click += new System.EventHandler(this.mnFileClose_Click);
            // 
            // mnFileSeparator1
            // 
            this.mnFileSeparator1.Index = 3;
            this.mnFileSeparator1.Text = "-";
            // 
            // mnFileSave
            // 
            this.mnFileSave.Index = 4;
            this.mnFileSave.Text = "&Uložit";
            this.mnFileSave.Click += new System.EventHandler(this.mnFileSave_Click);
            // 
            // mnFileSaveAs
            // 
            this.mnFileSaveAs.Index = 5;
            this.mnFileSaveAs.Text = "&Uložit jako...";
            this.mnFileSaveAs.Click += new System.EventHandler(this.mnFileSaveAs_Click);
            // 
            // mnFileSeparator2
            // 
            this.mnFileSeparator2.Index = 6;
            this.mnFileSeparator2.Text = "-";
            // 
            // mnExit
            // 
            this.mnExit.Index = 7;
            this.mnExit.Text = "&Konec";
            this.mnExit.Click += new System.EventHandler(this.mnExit_Click);
            // 
            // mnSettings
            // 
            this.mnSettings.Index = 1;
            this.mnSettings.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnSetttingsRegistry});
            this.mnSettings.Text = "&Nastavení";
            // 
            // mnSetttingsRegistry
            // 
            this.mnSetttingsRegistry.Index = 0;
            this.mnSetttingsRegistry.Text = "&Registrace ve Windows";
            this.mnSetttingsRegistry.Click += new System.EventHandler(this.mnSetttingsRegistry_Click);
            // 
            // mnWindow
            // 
            this.mnWindow.Index = 2;
            this.mnWindow.MdiList = true;
            this.mnWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MnCascade,
            this.mnTileHorizontal,
            this.mnTileVertical,
            this.mnArrangeIcons});
            this.mnWindow.Text = "&Okno";
            // 
            // MnCascade
            // 
            this.MnCascade.Index = 0;
            this.MnCascade.Text = "&Uspoøádat";
            this.MnCascade.Click += new System.EventHandler(this.MnCascade_Click);
            // 
            // mnTileHorizontal
            // 
            this.mnTileHorizontal.Index = 1;
            this.mnTileHorizontal.Text = "&Vodorovnì";
            this.mnTileHorizontal.Click += new System.EventHandler(this.mnTileHorizontal_Click);
            // 
            // mnTileVertical
            // 
            this.mnTileVertical.Index = 2;
            this.mnTileVertical.Text = "&Svisle";
            this.mnTileVertical.Click += new System.EventHandler(this.mnTileVertical_Click);
            // 
            // mnArrangeIcons
            // 
            this.mnArrangeIcons.Index = 3;
            this.mnArrangeIcons.Text = "Seøadit &ikony";
            this.mnArrangeIcons.Click += new System.EventHandler(this.mnArrangeIcons_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 713);
            this.IsMdiContainer = true;
            this.Menu = this.mnMenu;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mnMenu;
        private System.Windows.Forms.MenuItem mnFile;
        private System.Windows.Forms.MenuItem mnFileNew;
        private System.Windows.Forms.MenuItem mnFileOpen;
        private System.Windows.Forms.MenuItem mnFileSave;
        private System.Windows.Forms.MenuItem mnFileSaveAs;
        private System.Windows.Forms.MenuItem mnFileSeparator2;
        private System.Windows.Forms.MenuItem mnExit;
        private System.Windows.Forms.MenuItem mnWindow;
        private System.Windows.Forms.MenuItem MnCascade;
        private System.Windows.Forms.MenuItem mnTileHorizontal;
        private System.Windows.Forms.MenuItem mnTileVertical;
        private System.Windows.Forms.MenuItem mnArrangeIcons;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuItem mnFileClose;
        private System.Windows.Forms.MenuItem mnFileSeparator1;
        private System.Windows.Forms.MenuItem mnSettings;
        private System.Windows.Forms.MenuItem mnSetttingsRegistry;
    }
}